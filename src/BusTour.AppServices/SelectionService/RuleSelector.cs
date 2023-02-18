using BusTour.AppServices.SelectionService.Models.Actions;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Parsing;
using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;
using Infrastructure.Common.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService
{
    /// <summary>
    /// Класс для подбора лучшего правила.
    /// </summary>
    public class RuleSelector
    {
        private readonly List<RuleTypesTwoSeats> _ruleTypesTwoSeats = new List<RuleTypesTwoSeats>();
        private readonly List<RuleTypesFourSeats> _ruleTypesFourSeats = new List<RuleTypesFourSeats>();
        private readonly Dictionary<RuleTypesTwoSeats, Dictionary<RuleTypesFourSeats, YesNoRule>> _rules =
            new Dictionary<RuleTypesTwoSeats, Dictionary<RuleTypesFourSeats, YesNoRule>>();

        private ParseDebugModel _debugInfo;
        private readonly Dictionary<RuleTypesFourSeats, string> _debugDictionaryFourSeats = new Dictionary<RuleTypesFourSeats, string>();
        private readonly Dictionary<RuleTypesTwoSeats, string> _debugDictionaryTwoSeats = new Dictionary<RuleTypesTwoSeats, string>();

        /// <summary>
        /// Конструктор класса <see cref="RuleSelector"/>
        /// </summary>
        /// <param name="fileName">Имя файла с правилами.</param>
        public RuleSelector(string fileName)
        {
            ParsingErrors = Parse(fileName);
        }

        /// <summary>
        /// Список ошибок парсинга файла с правилами.
        /// </summary>
        public List<string> ParsingErrors { get; }

        /// <summary>
        /// Подбор лучшего правила.
        /// </summary>
        /// <param name="busModel">Модель автобуса.</param>
        /// <param name="path">Список шагов до лучшего правила.</param>
        /// <returns>Лучшее правило.</returns>
        public BaseRuleAction SelectBestRuleAction(BusModel busModel, out List<TestPathStep> path)
        {
            path = new List<TestPathStep>();

            if (!_ruleTypesTwoSeats.Any())
                return null;

            if (!_ruleTypesFourSeats.Any())
                return null;

            var checkResultsRuleTypesTwoSeats  = new Dictionary<RuleTypesTwoSeats,  bool>();
            var checkResultsRuleTypesFourSeats = new Dictionary<RuleTypesFourSeats, bool>();

            var steps = new List<Tuple<RuleTypesTwoSeats, RuleTypesFourSeats>>();

            var ruleTypeTwoSeats  = _ruleTypesTwoSeats.First();
            var ruleTypeFourSeats = _ruleTypesFourSeats.First();

            try
            {
                do
                {
                    steps.Add(new Tuple<RuleTypesTwoSeats, RuleTypesFourSeats>(ruleTypeTwoSeats, ruleTypeFourSeats));

                    var rule = SelectRule(ruleTypeTwoSeats, ruleTypeFourSeats);
                    if (rule == null)
                        return null;

                    if (!checkResultsRuleTypesTwoSeats.TryGetValue(ruleTypeTwoSeats, out var checkResultsTwoSeats))
                    {
                        checkResultsTwoSeats = busModel.CheckRuleType(ruleTypeTwoSeats);
                        checkResultsRuleTypesTwoSeats[ruleTypeTwoSeats] = checkResultsTwoSeats;
                    }

                    if (!checkResultsRuleTypesFourSeats.TryGetValue(ruleTypeFourSeats, out var checkResultsFourSeats))
                    {
                        checkResultsFourSeats = busModel.CheckRuleType(ruleTypeFourSeats);
                        checkResultsRuleTypesFourSeats[ruleTypeFourSeats] = checkResultsFourSeats;
                    }

                    var ruleAction = rule.GetAction(checkResultsTwoSeats, checkResultsFourSeats);
                    if (ruleAction == null)
                        return null;

                    path.Add(CreateStep(ruleTypeTwoSeats, ruleTypeFourSeats, checkResultsTwoSeats, checkResultsFourSeats, ruleAction));

                    if (ruleAction is RedirectAction redirectRuleAction)
                    {
                        ruleTypeTwoSeats  = redirectRuleAction.RedirectRuleTypesTwoSeats  ?? ruleTypeTwoSeats;
                        ruleTypeFourSeats = redirectRuleAction.RedirectRuleTypesFourSeats ?? ruleTypeFourSeats;
                    }
                    else
                    {
                        return ruleAction;
                    }

                    // Проверка на цикл.
                    if (steps.Any(p => p.Item1 == ruleTypeTwoSeats && p.Item2 == ruleTypeFourSeats))
                        return null;
                }
                while (true);
            }
            catch (Exception ex)
            {
                Log.For<TestBusModel>()
                    .LogError(ex, ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Парсинг файла с правилами.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Список ошибок парсинга.</returns>
        private List<string> Parse(string fileName)
        {
            var fileContent = Resources.EmbeddedResource.GetFileContent(fileName);

            try
            {
                var parseResult = JsonConvert.DeserializeObject<ParseModel>(fileContent);
                if (parseResult == null)
                    return new List<string> { $"Parsing file {fileName} error" };

                if (parseResult.RuleTypesTwoSeats != null)
                {
                    foreach (var ruleType in parseResult.RuleTypesTwoSeats)
                        RegisterRuleType(ruleType);
                }

                if (parseResult.RuleTypesFourSeats != null)
                {
                    foreach (var ruleType in parseResult.RuleTypesFourSeats)
                        RegisterRuleType(ruleType);
                }

                if (parseResult.Rules != null)
                {
                    foreach (var rule in parseResult.Rules)
                    {
                        var yesNoRule = CreateYesNoRule(rule);
                        AddRule(rule.RuleTypesTwoSeats, rule.RuleTypesFourSeats, yesNoRule);
                    }
                }

                _debugInfo = parseResult.Debug;

                char pos = 'A';
                foreach (var item in _ruleTypesFourSeats)
                {
                    _debugDictionaryFourSeats[item] = $"{pos}";
                    pos++;
                }

                pos = '1';
                foreach (var item in _ruleTypesTwoSeats)
                {
                    _debugDictionaryTwoSeats[item] = $"{pos}";
                    pos++;
                }

                return Check();
            }
            catch (Exception ex)
            {
                return new List<string> { ex.Message };
            }
        }

        /// <summary>
        /// Регистрация типа правила для столов на двоих.
        /// </summary>
        /// <param name="ruleType">Тип правила для столов нв двоих.</param>
        private void RegisterRuleType(RuleTypesTwoSeats ruleType)
        {
            if (_ruleTypesTwoSeats.Contains(ruleType))
                throw new Exception($"Тип {ruleType} уже зарегистрирован");

            _ruleTypesTwoSeats.Add(ruleType);
        }

        /// <summary>
        /// Регистрация типа правила для столов на четверых.
        /// </summary>
        /// <param name="ruleType">Тип правила для столов нв четверых.</param>
        private void RegisterRuleType(RuleTypesFourSeats ruleType)
        {
            if (_ruleTypesFourSeats.Contains(ruleType))
                throw new Exception($"Тип {ruleType} уже зарегистрирован");

            _ruleTypesFourSeats.Add(ruleType);
        }

        /// <summary>
        /// Добавление правила.
        /// </summary>
        /// <param name="ruleTypeTwoSeats">Тип правила для столов нв двоих.</param>
        /// <param name="ruleTypeFourSeats">Тип правила для столов нв четверых.</param>
        /// <param name="rule">Правило.</param>
        private void AddRule(RuleTypesTwoSeats ruleTypeTwoSeats, RuleTypesFourSeats ruleTypeFourSeats, YesNoRule rule)
        {
            if (!_rules.TryGetValue(ruleTypeTwoSeats, out var ruleList))
            {
                ruleList = new Dictionary<RuleTypesFourSeats, YesNoRule>();
                _rules[ruleTypeTwoSeats] = ruleList;
            }

            if (ruleList.ContainsKey(ruleTypeFourSeats))
                throw new Exception($"Правило [{ruleTypeTwoSeats}][{ruleTypeFourSeats}] уже зарегистрировано");

            ruleList[ruleTypeFourSeats] = rule;
        }

        /// <summary>
        /// Создание правила.
        /// </summary>
        /// <param name="rule">Распарсенное правило из файла.</param>
        /// <returns>Правило.</returns>
        private YesNoRule CreateYesNoRule(ParseRuleModel rule)
        {
            if (rule == null)
                return null;

            var result = new YesNoRule
            {
                ActionYesYes = CreateAction(rule.ActionYesYes),
                ActionYesNo  = CreateAction(rule.ActionYesNo),
                ActionNoYes  = CreateAction(rule.ActionNoYes),
                ActionNoNo   = CreateAction(rule.ActionNoNo)
            };

            return result;
        }

        /// <summary>
        /// Создание действия.
        /// </summary>
        /// <param name="action">Распарсенное действие из файла.</param>
        /// <returns>Действие.</returns>
        private BaseRuleAction CreateAction(ParseActionModel action)
        {
            if (action == null)
                return null;

            BaseRuleAction result = null;

            switch (action.Type)
            {
                case ActionTypes.Redirect:
                    result = new RedirectAction
                    {
                        RedirectRuleTypesTwoSeats = action.RedirectRuleTypesTwoSeats,
                        RedirectRuleTypesFourSeats = action.RedirectRuleTypesFourSeats
                    };
                    break;

                case ActionTypes.Table2:
                    result = new ProposalTableAction { TableType = ActionTableTypes.Two };
                    break;
                case ActionTypes.Table4:
                    result = new ProposalTableAction { TableType = ActionTableTypes.Four };
                    break;
                case ActionTypes.Table2and4:
                    result = new ProposalTableAction { TableType = ActionTableTypes.TwoAndFour };
                    break;

                case ActionTypes.Table4_GroupTables:
                    result = new Table4GroupTablesAction();
                    break;

                case ActionTypes.Seat1of2:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.Two, AvailableSeats2 = 1 };
                    break;
                case ActionTypes.Seat2of2:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.Two, AvailableSeats2 = 2 };
                    break;

                case ActionTypes.Seat1of4:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.Four, AvailableSeats4 = 1 };
                    break;
                case ActionTypes.Seat2of4:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.Four, AvailableSeats4 = 2 };
                    break;
                case ActionTypes.Seat3of4:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.Four, AvailableSeats4 = 3 };
                    break;
                case ActionTypes.Seat4of4:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.Four, AvailableSeats4 = 4 };
                    break;

                case ActionTypes.Seat1of2_Seat1of4:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.TwoAndFour, AvailableSeats2 = 1, AvailableSeats4 = 1 };
                    break;
                case ActionTypes.Seat1of2_Seat2of4:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.TwoAndFour, AvailableSeats2 = 1, AvailableSeats4 = 2 };
                    break;
                case ActionTypes.Seat1of2_Seat3of4:
                    result = new ProposalSeatAction { SeatType = ActionTableTypes.TwoAndFour, AvailableSeats2 = 1, AvailableSeats4 = 3 };
                    break;

                case ActionTypes.Seat1of2_Table4:
                    result = new ProposalSeat2Table4Action();
                    break;

                case ActionTypes.Seat2of4_Table2:
                    result = new ProposalSeat4Table2Action { AvailableSeats4 = 2 };
                    break;
                case ActionTypes.Seat3of4_Table2:
                    result = new ProposalSeat4Table2Action { AvailableSeats4 = 3 };
                    break;

                case ActionTypes.OtherOption:
                    result = new OtherOptionAction();
                    break;

                case ActionTypes.Unavailable:
                    result = new UnavailableAction();
                    break;
            }

            if (result != null)
            {
                result.Type = action.Type;
            }

            return result;
        }

        /// <summary>
        /// Проверка результата парсинга файла.
        /// </summary>
        /// <returns>Список ошибок парсинга.</returns>
        private List<string> Check()
        {
            var result = new List<string>();

            if (!_ruleTypesTwoSeats.Any())
                result.Add("Не задан ни один тип RuleTypesTwoSeats");

            if (!_ruleTypesFourSeats.Any())
                result.Add("Не задан ни один тип RuleTypesFourSeats");

            if (result.Any())
                return result;

            foreach (var ruleTypeTwoSeats in _ruleTypesTwoSeats)
            {
                if (!_rules.TryGetValue(ruleTypeTwoSeats, out var ruleList))
                {
                    result.Add($"Тип {ruleTypeTwoSeats} не зарегистрирован");
                    continue;
                }

                foreach (var ruleTypeFourSeats in _ruleTypesFourSeats)
                {
                    if (!ruleList.TryGetValue(ruleTypeFourSeats, out var rule))
                    {
                        result.Add($"Правило [{ruleTypeTwoSeats}][{ruleTypeFourSeats}] не зарегистрировано");
                    }

                    if (rule.IsEmpty)
                    {
                        result.Add($"Правило [{ruleTypeTwoSeats}][{ruleTypeFourSeats}] пустое");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Выбор правила по заданным типам.
        /// </summary>
        /// <param name="ruleTypeTwoSeats">Тип правила для столов нв двоих.</param>
        /// <param name="ruleTypeFourSeats">Тип правила для столов нв четверых.</param>
        /// <returns>Правило.</returns>
        private YesNoRule SelectRule(RuleTypesTwoSeats ruleTypeTwoSeats, RuleTypesFourSeats ruleTypeFourSeats)
        {
            if (!_rules.TryGetValue(ruleTypeTwoSeats, out var ruleList))
                throw new Exception($"Тип {ruleTypeTwoSeats} не зарегистрирован");

            if (!ruleList.TryGetValue(ruleTypeFourSeats, out var rule))
                throw new Exception($"Правило [{ruleTypeTwoSeats}][{ruleTypeFourSeats}] не зарегистрировано");

            return rule;
        }

        #region Test methods
        public TestResponseRulesTable GetRulesTable()
        {
            var result = new TestResponseRulesTable();

            if (_debugInfo == null || 
                _debugInfo.RuleTypesTwoSeats == null || 
                _debugInfo.RuleTypesFourSeats == null || 
                _debugInfo.Actions == null)
                return result;

            var row = new TestResponseRulesRow();

            row.Cells.Add(
                new TestResponseRulesCell
                {
                    RowSpan    = 3,
                    ColSpan    = 3,
                    Text       = _debugInfo.Name,
                    Background = "white",
                    Color      = "black",
                    IsBold     = true
                });

            foreach (var rule in _ruleTypesFourSeats)
            {
                row.Cells.Add(
                    new TestResponseRulesCell
                    {
                        ColSpan    = 2,
                        Text       = _debugDictionaryFourSeats.TryGetValue(rule, out var text) ? text : null,
                        Background = "white",
                        Color      = "black",
                        IsBold     = true
                    });
            }

            result.Rows.Add(row);


            row = new TestResponseRulesRow();

            foreach (var rule in _ruleTypesFourSeats)
            {
                var cell = CreateCell(rule);
                cell.ColSpan = 2;
                cell.IsBold = true;
                row.Cells.Add(cell);
            }

            result.Rows.Add(row);

            row = new TestResponseRulesRow();

            foreach (var rule in _ruleTypesFourSeats)
            {
                row.Cells.Add(
                    new TestResponseRulesCell
                    {
                        Text       = "Есть",
                        Background = "white",
                        Color      = "black"
                    });
                row.Cells.Add(
                    new TestResponseRulesCell
                    {
                        Text       = "Нет",
                        Background = "white",
                        Color      = "black"
                    });
            }

            result.Rows.Add(row);

            foreach (var rule2 in _ruleTypesTwoSeats)
            {
                row = new TestResponseRulesRow();

                row.Cells.Add(
                    new TestResponseRulesCell
                    {
                        RowSpan    = 2,
                        Text       = _debugDictionaryTwoSeats.TryGetValue(rule2, out var text) ? text : null,
                        Background = "white",
                        Color      = "black",
                        IsBold     = true
                    });

                var cell = CreateCell(rule2);
                cell.RowSpan = 2;
                cell.IsBold = true;
                row.Cells.Add(cell);

                row.Cells.Add(
                    new TestResponseRulesCell
                    {
                        Text       = "Есть",
                        Background = "white",
                        Color      = "black"
                    });

                foreach (var rule4 in _ruleTypesFourSeats)
                {
                    var yesNoRule = SelectRule(rule2, rule4);

                    var action = yesNoRule.GetAction(true, true);
                    row.Cells.Add(CreateCell(action));

                    action = yesNoRule.GetAction(true, false);
                    row.Cells.Add(CreateCell(action));
                }

                result.Rows.Add(row);

                row = new TestResponseRulesRow();

                row.Cells.Add(
                    new TestResponseRulesCell
                    {
                        Text       = "Нет",
                        Background = "white",
                        Color      = "black"
                    });

                foreach (var rule4 in _ruleTypesFourSeats)
                {
                    var yesNoRule = SelectRule(rule2, rule4);

                    var action = yesNoRule.GetAction(false, true);
                    row.Cells.Add(CreateCell(action));

                    action = yesNoRule.GetAction(false, false);
                    row.Cells.Add(CreateCell(action));
                }

                result.Rows.Add(row);
            }

            int rowNumber = 1;
            int cellNumber = 1;

            foreach (var resultRow in result.Rows)
            {
                resultRow.Code = $"row_{_debugInfo.Code}_{rowNumber}";

                foreach (var resultCell in resultRow.Cells)
                {
                    resultCell.Code = $"cell_{_debugInfo.Code}_{cellNumber}";
                    cellNumber++;
                }

                rowNumber++;
            }

            return result;
        }

        public TestResponseRulesTable GetRulesLegend()
        {
            var result = new TestResponseRulesTable();

            if (_debugInfo == null || _debugInfo.Actions == null)
                return result;

            foreach (var action in _debugInfo.Actions)
            {
                var row = new TestResponseRulesRow();

                row.Cells.Add(CreateCell(action));

                row.Cells.Add(
                    new TestResponseRulesCell
                    {
                        Text        = action.Description,
                        Background  = "white",
                        Color       = "black",
                        IsLeftAlign = true
                    });

                result.Rows.Add(row);
            }

            int rowNumber = 1;
            int cellNumber = 1;

            foreach (var resultRow in result.Rows)
            {
                resultRow.Code = $"legend_row_{_debugInfo.Code}_{rowNumber}";

                foreach (var resultCell in resultRow.Cells)
                {
                    resultCell.Code = $"legend_cell_{_debugInfo.Code}_{cellNumber}";
                    cellNumber++;
                }

                rowNumber++;
            }

            return result;
        }

        private TestResponseRulesCell CreateCell(RuleTypesTwoSeats rule)
        {
            var info = _debugInfo.RuleTypesTwoSeats.TryGetValue(rule, out var value) ? value : null;

            return CreateCell(info);
        }

        private TestResponseRulesCell CreateCell(RuleTypesFourSeats rule)
        {
            var info = _debugInfo.RuleTypesFourSeats.TryGetValue(rule, out var value) ? value : null;

            return CreateCell(info);
        }

        private TestResponseRulesCell CreateCell(ParseDebugItem item)
        {
            var result = new TestResponseRulesCell
            {
                Text = item?.Text,
                Color = item?.Color,
                Background = item?.Background
            };

            return result;
        }

        private TestResponseRulesCell CreateCell(BaseRuleAction action)
        {
            if (action == null)
                return new TestResponseRulesCell();

            var info = _debugInfo.Actions.FirstOrDefault(p => p.Types.Contains(action.Type));

            var result = CreateCell(info);

            var redirectText = GetRedirectActionText(action as RedirectAction);
            if (!string.IsNullOrWhiteSpace(redirectText))
                result.Text = redirectText;

            return result;
        }

        private TestPathStep CreateStep(RuleTypesTwoSeats ruleTypeTwoSeats, RuleTypesFourSeats ruleTypeFourSeats, bool checkResultsTwoSeats, bool checkResultsFourSeats, BaseRuleAction ruleAction)
        {
            var redirectText = GetRedirectActionText(ruleAction as RedirectAction);

            var actionText =
                !string.IsNullOrWhiteSpace(redirectText)
                    ? redirectText
                    : _debugInfo.Actions.FirstOrDefault(p => p.Types.Contains(ruleAction.Type))?.Description;

            var ruleInfo2 = _debugInfo.RuleTypesTwoSeats.TryGetValue(ruleTypeTwoSeats,   out var item2) ? item2 : null;
            var ruleInfo4 = _debugInfo.RuleTypesFourSeats.TryGetValue(ruleTypeFourSeats, out var item4) ? item4 : null;

            var result = new TestPathStep
            {
                Position        = GetPosition(ruleTypeTwoSeats, ruleTypeFourSeats),
                CheckTwo        = ruleInfo2?.Text,
                CheckFour       = ruleInfo4?.Text,
                CheckResultTwo  = checkResultsTwoSeats  ? "Есть" : "Нет",
                CheckResultFour = checkResultsFourSeats ? "Есть" : "Нет",
                Action          = actionText,
            };

            return result;
        }

        private string GetRedirectActionText(RedirectAction actionRedirect)
        {
            if (actionRedirect == null)
                return null;

            var text = GetPosition(actionRedirect.RedirectRuleTypesTwoSeats, actionRedirect.RedirectRuleTypesFourSeats);

            return !string.IsNullOrWhiteSpace(text) ? $"переход на {text}" : null;
        }

        private string GetPosition(RuleTypesTwoSeats? ruleTypeTwoSeat, RuleTypesFourSeats? ruleTypeFourSeat)
        {
            var row =
                ruleTypeTwoSeat.HasValue && _debugDictionaryTwoSeats.TryGetValue(ruleTypeTwoSeat.Value, out var value2)
                    ? value2
                    : null;

            var col =
                ruleTypeFourSeat.HasValue && _debugDictionaryFourSeats.TryGetValue(ruleTypeFourSeat.Value, out var value4)
                    ? value4
                    : null;

            var result = $"{row}{col}";

            return result;
        }
        #endregion
    }
}

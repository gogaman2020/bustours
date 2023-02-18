using BusTour.Domain.Enums;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;
using Infrastructure.Common.DI;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService
{
    /// <summary>
    /// Сервис подбора мест.
    /// </summary>
    [InjectAsSingleton]
    public class TestSelectionService : ITestSelectionService
    {
        private readonly RuleSelector _rsTable1 = new RuleSelector("Rules_1_Table");
        private readonly RuleSelector _rsSeat1  = new RuleSelector("Rules_1_Seat");
        private readonly RuleSelector _rsTable2 = new RuleSelector("Rules_2_Table");
        private readonly RuleSelector _rsSeat2  = new RuleSelector("Rules_2_Seat");
        private readonly RuleSelector _rsTable3 = new RuleSelector("Rules_3_Table");
        private readonly RuleSelector _rsTable4 = new RuleSelector("Rules_4_Table");

        private readonly Dictionary<int, RuleSelectorWrapper> _ruleSelectorWrappersTable =
            new Dictionary<int, RuleSelectorWrapper>();
        private readonly Dictionary<int, RuleSelectorWrapper> _ruleSelectorWrappersSeat =
            new Dictionary<int, RuleSelectorWrapper>();

        private readonly Dictionary<SelectionVariant, ILookup<int, RuleSelectorWrapper>> _ruleSelectors =
            new Dictionary<SelectionVariant, ILookup<int, RuleSelectorWrapper>>();

        public TestSelectionService()
        {
            Init();
        }

        public TestSelectionResult Select(TestBusModel busModel, SelectionInfo selectionInfo, bool isLockMode = false)
        {
            busModel.ClearRecommended();

            if (selectionInfo.ClickedObject == null)
            {
                selectionInfo.SelectedObjects?.Clear();

                if (TryAutoSelect(busModel, selectionInfo, out var autoSelectPath))
                {
                    if (selectionInfo.ManualSelectionMode)
                    {
                        busModel.SaveSelectedToRecommended();
                        selectionInfo.SelectedObjects?.Clear();
                        busModel.UnSelectAll();

                        selectionInfo.ManualSelectionMode = false;
                    }
                    else
                    {
                        return new TestSelectionResult
                            {
                                IsAutoSelect = true,
                                AvailableObjects = new List<BusObject>(),
                                Path = autoSelectPath
                            };
                    }
                }
                else
                {
                    selectionInfo.SelectedObjects?.Clear();
                    busModel.UnSelectAll();
                }
            }
            else
            {
                if (isLockMode)
                {
                    selectionInfo.SelectedObjects?.Clear();
                    busModel.LockSwitch(selectionInfo.ClickedObject);
                }
                else
                {
                    busModel.ApplySelection(selectionInfo.SelectedObjects);
                    busModel.SelectionSwitch(selectionInfo);
                }
            }

            var ruleSelector = GetRuleSelector(busModel, selectionInfo);
            if (ruleSelector == null)
                return new TestSelectionResult();

            var ruleAction = ruleSelector.SelectBestRuleAction(busModel, out var path);

            var availableObjects = new List<BusObject>();

            var neededSeats = selectionInfo.GuestCount - busModel.CountSelectedSeats;

            if (neededSeats > 0)
            {
                availableObjects = ruleAction.GetAvailableObjects(busModel, neededSeats);

                var tables =
                    busModel.Tables
                        .Where(p => 
                            p.CountSelectedSeats > 0 && 
                            availableObjects.Any(x => x.Type == BusObjectTypes.Seat && p.Seats.Any(s => s.Id == x.Id)))
                        .ToArray();

                if (tables.Any())
                {
                    availableObjects
                        .RemoveAll(p => p.Type == BusObjectTypes.Table || !tables.Any(x => x.Seats.Any(s => s.Id == p.Id)));
                }
            }

            return new TestSelectionResult
                {
                    AvailableObjects = availableObjects,
                    Path = path
                };
        }

        public TestResponseDebugInfo GetRulesDebugInfo(TestBusModel busModel, SelectionInfo selectionInfo)
        {
            busModel.ApplySelection(selectionInfo.SelectedObjects);

            var ruleSelector = GetRuleSelector(busModel, selectionInfo);

            var result = new TestResponseDebugInfo
            {
                Table = ruleSelector?.GetRulesTable() ?? new TestResponseRulesTable(),
                Legend = ruleSelector?.GetRulesLegend() ?? new TestResponseRulesTable(),
            };

            return result;
        }

        private void Init()
        {
            for (var pos = 1; pos <= 8; pos++)
            {
                _ruleSelectorWrappersTable[pos] = new RuleSelectorWrapper();
                _ruleSelectorWrappersSeat[pos]  = new RuleSelectorWrapper();
            }

            _ruleSelectorWrappersTable[1].Add(0, null, _rsTable1);
            _ruleSelectorWrappersTable[2].Add(0, null, _rsTable2);
            _ruleSelectorWrappersTable[3].Add(0, null, _rsTable3);
            _ruleSelectorWrappersTable[4].Add(0, null, _rsTable4);
            _ruleSelectorWrappersTable[5].Add(0, 3, _rsTable4);
            _ruleSelectorWrappersTable[5].Add(4, null, _rsTable1);
            _ruleSelectorWrappersTable[6].Add(0, 3, _rsTable4);
            _ruleSelectorWrappersTable[6].Add(4, null, _rsTable2);
            _ruleSelectorWrappersTable[7].Add(0, 3, _rsTable4);
            _ruleSelectorWrappersTable[7].Add(4, null, _rsTable3);
            _ruleSelectorWrappersTable[8].Add(0, 3, _rsTable4);
            _ruleSelectorWrappersTable[8].Add(4, null, _rsTable4);

            _ruleSelectorWrappersSeat[1].Add(0, null, _rsSeat1);
            _ruleSelectorWrappersSeat[2].Add(0, null, _rsSeat2);
            _ruleSelectorWrappersSeat[3].Add(0, 1, _rsTable2); // 3 - Table2 + Seat1
            _ruleSelectorWrappersSeat[3].Add(2, null, _rsSeat1);
            _ruleSelectorWrappersSeat[4].Add(0, 1, _rsTable2); // 4 - Table2 + Table2
            _ruleSelectorWrappersSeat[4].Add(2, null, _rsTable2);
            _ruleSelectorWrappersSeat[5].Add(0, 1, _rsTable2); // 5 - Table2 + Table2 + Seat1
            _ruleSelectorWrappersSeat[5].Add(2, 3, _rsTable2);
            _ruleSelectorWrappersSeat[5].Add(4, null, _rsSeat1);
            _ruleSelectorWrappersSeat[6].Add(0, 1, _rsTable2); // 6 - Table2 + Table2 + Table2
            _ruleSelectorWrappersSeat[6].Add(2, 3, _rsTable2);
            _ruleSelectorWrappersSeat[6].Add(4, null, _rsTable2);
            _ruleSelectorWrappersSeat[7].Add(0, 1, _rsTable2); // 7 - Table2 + Table2 + Table2 + Seat1
            _ruleSelectorWrappersSeat[7].Add(2, 3, _rsTable2);
            _ruleSelectorWrappersSeat[7].Add(4, 5, _rsTable2);
            _ruleSelectorWrappersSeat[7].Add(6, null, _rsSeat1);
            _ruleSelectorWrappersSeat[8].Add(0, 1, _rsTable2); // 8 - Table2 + Table2 + Table2 + Table2
            _ruleSelectorWrappersSeat[8].Add(2, 3, _rsTable2);
            _ruleSelectorWrappersSeat[8].Add(4, 5, _rsTable2);
            _ruleSelectorWrappersSeat[8].Add(6, null, _rsTable2);

            _ruleSelectors[SelectionVariant.IndividualTable] = new[]
                {
                    new { Count = 1, Wrapper = _ruleSelectorWrappersTable[1] },
                    new { Count = 2, Wrapper = _ruleSelectorWrappersTable[2] },
                    new { Count = 3, Wrapper = _ruleSelectorWrappersTable[3] },
                    new { Count = 4, Wrapper = _ruleSelectorWrappersTable[4] },
                    new { Count = 5, Wrapper = _ruleSelectorWrappersTable[5] },
                    new { Count = 6, Wrapper = _ruleSelectorWrappersTable[6] },
                    new { Count = 7, Wrapper = _ruleSelectorWrappersTable[7] },
                    new { Count = 8, Wrapper = _ruleSelectorWrappersTable[8] },
                }.ToLookup(p => p.Count, p => p.Wrapper);

            _ruleSelectors[SelectionVariant.SharedTable] = new[]
                {
                    new { Count = 1, Wrapper = _ruleSelectorWrappersSeat[1]  },
                    new { Count = 2, Wrapper = _ruleSelectorWrappersSeat[2]  },
                    new { Count = 3, Wrapper = _ruleSelectorWrappersTable[3] },
                    new { Count = 3, Wrapper = _ruleSelectorWrappersSeat[3]  },
                    new { Count = 4, Wrapper = _ruleSelectorWrappersTable[4] },
                    new { Count = 4, Wrapper = _ruleSelectorWrappersSeat[4]  },
                    new { Count = 5, Wrapper = _ruleSelectorWrappersTable[5] },
                    new { Count = 5, Wrapper = _ruleSelectorWrappersSeat[5]  },
                    new { Count = 6, Wrapper = _ruleSelectorWrappersTable[6] },
                    new { Count = 6, Wrapper = _ruleSelectorWrappersSeat[6]  },
                    new { Count = 7, Wrapper = _ruleSelectorWrappersTable[7] },
                    new { Count = 7, Wrapper = _ruleSelectorWrappersSeat[7]  },
                    new { Count = 8, Wrapper = _ruleSelectorWrappersTable[8] },
                    new { Count = 8, Wrapper = _ruleSelectorWrappersSeat[8]  }
                }.ToLookup(p => p.Count, p => p.Wrapper);
        }

        private RuleSelector GetRuleSelector(TestBusModel busModel, SelectionInfo selectionInfo)
        {
            var countSelectedSeats = busModel.CountSelectedSeats;

            if (!_ruleSelectors.TryGetValue(selectionInfo.SeatingType, out var wrappers))
                return null;

            var list = wrappers[selectionInfo.GuestCount].ToList();
            if (list == null)
                return null;

            RuleSelector result = null;

            foreach (var wrapper in list)
            {
                if (wrapper == null) continue;

                result = wrapper.GetSelector(countSelectedSeats);
                var ruleAction = result?.SelectBestRuleAction(busModel, out var path);

                if (ruleAction == null) continue;

                if (!ruleAction.NoProposals)
                    break;
            }

            return result;
        }

        private bool TryAutoSelect(TestBusModel busModel, SelectionInfo selectionInfo, out List<TestPathStep> path)
        {
            path = new List<TestPathStep>();

            while (true)
            {
                var ruleSelector = GetRuleSelector(busModel, selectionInfo);
                if (ruleSelector == null)
                    return false;

                var ruleAction = ruleSelector.SelectBestRuleAction(busModel, out path);
                if (ruleAction == null || ruleAction.NoProposals)
                    return false;

                if (!ruleAction.TryAutoSelect(busModel, selectionInfo))
                    return false;

                if (selectionInfo.GuestCount <= busModel.CountSelectedSeats)
                    return true;
            }
        }
    }
}

import createNumberMask from 'text-mask-addons/dist/createNumberMask';

export const integerVueMask = createNumberMask({
    prefix: '',
    allowDecimal: false,
    includeThousandsSeparator: false,
    allowNegative: false,
  });

export const currencyVueMask = createNumberMask({
  prefix: '',
  allowDecimal: true,
  includeThousandsSeparator: false,
  allowNegative: false,
});

export const percentVueMask = createNumberMask({
    prefix: '',    
    suffix: '',
    allowDecimal: false,
    includeThousandsSeparator: false,
    allowNegative: false,
    integerLimit: 2
  });

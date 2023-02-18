import { BaseEntity } from "../common";

// Промокод
export class Promocode {
	// Id
	public id: number = 0;

	// Серийный номер промо кода
	public seriesNumber: string = "";

	// Тип промокода
	public promoCodeType: PromocodeType | string = PromocodeType.Time.toString();

	// Дата начала действия
	public dateStart?: Date;

	// Дата окончания действия
	public dateEnd?: Date;

	// Количество использований промокода
	public numberOfPromocodes?: number;

	// Скидка
	public amountOfDiscount?: number;

	// Тип скидки
	public typeOfDiscount: DiscountType | string =  DiscountType.Percent.toString();

	public cityId?: number;

	public isActive?: boolean;

	//Невалидный промокод
	public isBadPromoCode?: boolean;
}

// Типы промокодов
export enum PromocodeType {
	// Временной (действует в заданном периоде)
	Time = 1,

	// Количественный (действует заданное число раз)
	Quantity = 2,

	// Временной и количественный (действует в заданном периоде определенное количество раз)
	TimeAndQuantity = 3,
}

// Тип скидки
export enum DiscountType {
	// В процентах
	Percent = 1,

	// Конкретная сумма в валюте
	Cash = 2,
}

export class PromoCodeFilter {
	city?: string;
	isActive: boolean = false;
	isExpiredOrAmended: boolean = false;
}

export interface IPromocodeGridModel extends BaseEntity {
	numberOfPromocodes?: number
	seriesNumber: string
	discountType: DiscountType
	amountOfDiscount: number
	creationDate: Date
	dateStart?: Date 
	expirationDate?: Date
	quantityIssued: string
	quantityUsed: number
	discountPlanned?: number
	discountUsed?: number
}
export interface PromoCodeValidateFilter {
    seriesNumber: string;
    cityId: number;
}

import { Time, Dictionary } from '@/types/common'
import { Order, OrderState, Client } from './booking';

export class ToursCreation {
    dateStart?: Date;
    dateEnd?: Date;
    unavailableDates?: Date[];
    chooseWeekdays?: boolean;
    type?: string;
    tours: TourCreation[] = [];
    roteId?: number; 
}

export class TourCreation {
    weekdayStart?: number;
    weekdayEnd?: number;
    times?: (Time | null)[] = [];
    price?: number;
    seatPrice?: number;
    vipPrice?: number; 
    discount?: number;
    hasMenu: boolean = true;
    hasBeverages: boolean = true;
    menus: number[] = [];
    beverages: number[] = [];
    menusExtra: number[] = [];
    beveragesExtra: number[] = [];
    serviceStart?: Time;
    serviceEnd?: Time;
}
export interface TourFB {
    toursInfo: TourInfo [],
    menus: OrderTourMenu [],
    beverages: OrderTourBeverage [],
    extraMenus: OrderTourMenu [],
    extraBeverages: OrderTourBeverage [],
    privateTourInfo: PrivateTourInfo[]
}

export interface PrivateTourInfo {
    id: number;
    comment: string;
    specialRequests: string;
}

export interface Tour {
    id: number;
    departure: Date| undefined;
    city: Dictionary<string>| undefined;
    tourType: number | undefined;
    itinerary: Dictionary<string>| undefined;
    status: string;
    duration: Time| undefined;
}

export interface TourInfo {
    id: number;
    departure: Date| undefined;
    city: Dictionary<string>;
    tourType?: TourType;
    itinerary: Dictionary<string>;
    status?: TourState;
    duration: string;
    guestsNumber: number;
    number: string;
}

export interface OrderTourMenu {
    menuId: number;
    tourId: number;
    name: Dictionary<string>| undefined;
    amount: number;
}

export interface MenuTourAmount {
    menuId: number;
    tourAmount: TourAmount[];
    name: Dictionary<string>| undefined;
}

export interface TourAmount {
    tourId: number;
    amount: number;
}

export interface OrderTourBeverage {
    beverageId: number;
    tourId: number;
    name: Dictionary<string>| undefined;
    volume: number| undefined;
    unit: Dictionary<string>| undefined;
    amount: number;
}

export interface CurrentBooking {
    id: number;
    departure: Date;
    tourType: TourType;
    itinerary: Dictionary<string>;
    privateHireComment: string;
    tourState?: TourState, 
    duration: string;
    guestsNumber: number;
    seatsNumber: number;
    conflicts: boolean;
    extras: boolean;
    paid: number;
    waiting: number;
    hasGroupOrder: boolean;
}

export interface City {
    id: number;
    name: Dictionary<string>;
}

export interface TourFilter {
    ids: number[]|undefined;
    cityId?: number|undefined;
    routeId? : number|undefined;
    departureDateFrom?: Date|undefined;
    departureDateTo?: Date|undefined;
    tourTypes: number[]|undefined;
    withoutOrders: boolean|undefined;
    hasOrders: boolean|undefined;
    date?: string|undefined;
    states?: TourState[];
}
export interface TourMenuBeverage {
    id: number | undefined;
    tourMenus: TourMenu[];
    tourBeverages: TourBeverage[];
}

export interface TourMenu {
    tourId: number;
    menuId: number;
    isTicket: boolean;
    isExtra: boolean;
}

export interface TourBeverage {
    tourId: number;
    beverageId: number;
    isTicket: boolean;
    isExtra: boolean;
}

export enum TourType {
    Regular = 0,
    PrivateHire = 20,
    Service = 30,
}

export enum TourState {
    
    /// Черновик
    Draft = 10,

    /// Активный
    Active = 20,

    /// Запрос на отмену
    CancelRequest = 30,

    /// Отменен
    Canceled = 40,

    /// Удален
    Deleted = 99,
}

export interface ITourSummary {
    tourId: number, 
    cityId: number, 
    city: Dictionary<string>;
    tourType: TourType, 
    itinerary: Dictionary<string>, 
    tourState?: TourState, 
    publicBookingBlock: boolean, 
    departureDateTime: Date, 
    arrivalDateTime: Date, 
    duration: string, 
    conflict: boolean, 
    occupaid?: number, 
    usedGiftsCount?: number, 
    userPromoCodesCount?: number, 
    extras: boolean,
    tourPaymentInformation: number,
}
export interface ITourOrderGridModel {
    orderId: number;
    client: Client;
    orderState?: OrderState;
    conflict: boolean;
}

export interface ITourConflictGridModel {
    conflictOrder: Order;
    conflictSeatIds: number[]
}

export interface ITourInformation {
    tourSummary: ITourSummary
    tourOrders: ITourOrderGridModel[]
    tourConflicts: ITourConflictGridModel[]
}

export interface TourPrivateHire {
    guestCount: number;
    blockBookingForDraft: boolean;
    blockBookingDateFrom: Date;
    blockBookingDateTo: Date;
    departurePoint: string;
    arrivalPoint: string;
    routeId: number | null;
    stops: string[];
    price: number;
}
import { Time } from "../common";
import { TourType, TourPrivateHire } from "../tour";
import { GiftCertificate } from "~/types/giftCertificate";
import { Promocode } from "~/types/promocodes/index"
export interface Dictionary<T> {
  [Key: string]: T;
}

export enum OrderState {
  Draft = 10,
  WaitingForPayment = 20,
  Paid = 30,
  NotPaid = 40, 
  Canceled = 50,
}

export enum OrderStep {
  Form = 0,
  Seats = 1,
  Menu = 2,
  Confirmation = 3,
  Payment = 4,
  Receipt = 5
}

export enum SeatingType {
  SeparateTable = 1,
  BySeats = 2
}

export enum TableCategoryEnum {
  Regular = 1,
  Vip = 2,
  Wheelchair = 3
}

export enum MenuTypeEnum {
  Main = 1,
  Special = 2,
  Dessert = 3,
}

export enum BusObjectTypes {
  Table = 1,
  Seat = 2
}

export interface RootState {
  languages: Language[];
  selectedLanguageCode: string;
  selectedRouteId: number;
  routeInfo: RouteInfo;
  selectionModel: ResponseSelection;
  hoveredSchemeItem: BusSchemeItem;
  menuInfo: MenuInfo;
  order: Order;
  orderSeats: OrderSeat[],
  orderExtras: OrderExtras,
  orderCertificate: GiftCertificate,
  orderPromocode: Promocode;
  tour: Tour;
  actionResult: BaseResponse;
  testInfo: TestInfo;
  debugInfo: DebugInfo;
  payment: Payment;
  buses: Bus[];
  calculationCostTour: CalculationCostTourResponse;
  isUpgradeMode: Boolean,
  schemeTables: SchemeTableModel[],
}

export interface Language {
  code: string;
  name: string;
}

export interface RouteInfo {
  route: Route;
  tours: Tour[];
}

export interface Route {
  id: number;
  name: Dictionary<string>;
  description: Dictionary<string>;
  duration: string;
  titleImgPath: string;
  mapImgPath: string;
  cityName: Dictionary<string>;
  departureAddress: Dictionary<string>;
  departureHowToGet: Dictionary<string>;
  imgPaths: string[];
  cityId: number;
}

export interface Tour {
  id: number;
  departure: string;
  arrival: string;
  tables: Table[];
  routeId: number | null;
  privateHire: TourPrivateHire | null;
  busId: number;
  isAvailableForBooking: boolean;
  occupiedSeatsCount: number;
  vipPrice?: number;
  seatPrice?: number;
}

export interface Table {
  id: number;
  number: Number;
  isAvailable: boolean;
  x: number;
  y: number;
  xSize: number;
  ySize: number;
  isLeft: boolean;
  isRight: boolean;
  floor: number;
  price: number;
  category: TableCategory;
  seats: Seat[];
  isVip: boolean;
}

export interface TableCategory {
  id: number;
  name: Dictionary<string>;
}

export interface Seat {
  id: number;
  name: string;
  x: number;
  y: number;
  isForward: boolean;
  isBackward: boolean;
  price: number;
  tableId?: number;
  rotate: number;
  type: SeatType;
  scaleX: number;
  scaleY: number;
}

export enum SeatType {
    Default = 1,
    Side = 2,
    Long = 3,
    Double = 4,
    Disabled = 5
}

export interface Bus {
    name: Dictionary<string>;
    tables: Table[];
}

export interface BusSchemeItem {
  seat: Seat | undefined;
  table: Table | undefined;
}

export interface MenuInfo {
  menus: Menu[];
  beverages: Beverage[];
  allergies: Allergy[];
  surprises: Surprise[];
}

export interface Menu {
  id: number;
  name: Dictionary<string>;
  price: number;
  imgPath: string;
  volume: number;
  unit: Dictionary<string>;
  menuType: MenuType;
}

export interface MenuType {
  id: number;
  name: Dictionary<string>;
}

export interface Beverage {
  id: number;
  name: Dictionary<string>;
  price: number;
  imgPath: string;
  group: BeverageGroup;
  volume: number;
  unit: Dictionary<string>;
  alcoholByVolume?: number;
  isHot: boolean;
  wineType?: WineType;
}

export interface BeverageGroup {
  id: number;
  name: Dictionary<string>;
}

export interface WineType {
  id: number;
  name: Dictionary<string>;
}

export interface Surprise {
  id: number;
  name: Dictionary<string>;
  price: number;
  imgPath: string;
}
export interface Allergy {
  id: number;
  name: Dictionary<string>
}

export interface Order {
  id: number;
  hash?: string;
  orderState: OrderState;
  step: OrderStep;
  type: OrderType;
  date?: Date;
  time?: Time;
  guestCount: number;
  guestsWithDisabilities: boolean;
  disabledGuestCount: number;
  seatingType: SeatingType;
  tourId: number;
  routeId: number | null,
  tour: Tour | null;
  promoCodeId?: number;
  promoCode?: string;
  certificateId?: string;
  certificateNumber?: string;
  giftCertificate?: GiftCertificate,
  discount?: number;
  totalPrice: number;
  client: Client;
  seats: OrderSeat[];
  tables: OrderTable[];
  menus: OrderMenu[];
  beverages: OrderBeverage[];
  surprises: OrderSurprise[];
  specialRequests: string | null;
  comment: string | null;
  privateHire: OrderPrivateHire;
  payment: Payment;
  isGroup: boolean;
  conflictsResponse: OrdersConflictsResponse | null;
}

export interface Client {
  id: number;
  fullName?: string;
  email: string;
  phoneNumber?: string;
  isSigned: boolean;
}

export interface OrderSeat {
  id: number;
  orderId: number;
  seatId: number;
  menuId: number | undefined;
  beverageId: number | undefined;
  isAllergy: boolean,
  allergyId: number | undefined,
  seatFullName: string | '';
  otherAllergy: string | undefined;
  guestHasCome: boolean,
  hasMenuIssued: boolean,
  hasBeverageIssued: boolean,
  rotate: number | 0,
  type: SeatType | 0,
  scaleX: number | 0,
  scaleY: number | 0,
}
export interface OrderTable {
  tableId: number;
}

export interface OrderMenu {
  id: number;
  orderId: number;
  menuId: number | undefined;
  amount: number;
  issued: boolean;
}
export interface OrderBeverage {
  id: number;
  orderId: number;
  beverageId: number;
  amount: number;
  issued: boolean;
}

export interface OrderSurprise {
  surpriseId: number;
  amount: number;
}

export interface BaseResponse {
  isSuccess: boolean;
  message?: string;
}

export interface OrderResponse extends BaseResponse {
  orderId: number;
  hash: string;
}

export interface TestSeatInfo {
  id: number;
  number: string;
  locked: boolean;
  selected: boolean;
  isRecommended: boolean;
  available: boolean;
}

export interface TestInfo {
  selectionResult: TestSelectionResultInfo;
  selection: SelectionInfo;
  tables: Dictionary<TestTableInfo>;
}

export interface TestSelectionResultInfo {
  isAutoSelect: boolean;
  path: TestStepInfo[];
}

export interface TestStepInfo {
  position: string;
  checkTwo: string;
  checkFour: string;
  checkResultTwo: string;
  checkResultFour: string;
  action: string;
}

export interface TestTableInfo {
  id: number;
  number: string;
  locked: boolean;
  selected: boolean;
  available: boolean;
  seats: Dictionary<TestSeatInfo>;
}

export interface TestSeatPosition {
  tableId: number;
  seatId: number | null;
}

export interface DebugInfo {
  table: DebugRulesTable;
  legend: DebugRulesTable;
}

export interface DebugRulesTable {
  rows: DebugRulesRow[];
}

export interface DebugRulesRow {
  code: string;
  cell: DebugRulesCell[];
}

export interface DebugRulesCell {
  code: string;
  rowSpan: number;
  colspan: number;
  text: string;
  color: string;
  background: string;
  isBold: boolean;
}

export interface Payment {
  cardholdersName: string,
  cardNumber: string,
  cardVerificationCode: string,
  name: string,
  email: string,
  repeatemail: string,
  mounth: number,
  year: number,
  details: PaymentDetails,
  phone: Phone
}

export interface Phone
{
  code: string;
  number: string
}

export interface PaymentDetails {
    error: string
}

export interface Extras {
  id?: number,
  name: Dictionary<string>,
  amount: number,
  price: number
}

export interface ResponseSelection extends BaseResponse {
  selection: SelectionInfo;
  tables: ResponseTable[];
  orderInfo: OrderInfo;
}

export interface SelectionInfo {
  tourId: number;
  guestCount: number;
  seatingType: SeatingType;
  selectedObjects: BusObject[];
  clickedObject: BusObject | null;
  manualSelectionMode: boolean;
  orderType: OrderType;
}

export interface BusObject {
  type: BusObjectTypes;
  id: number;
}

export interface ResponseTable {
  id: number;
  number: string;
  isLocked: boolean;
  isSelected: boolean;
  isAvailable: boolean;
  isFirstRow: boolean;
  isLastRow: boolean;
  seats: ResponseSeat[];
}

export interface ResponseSeat {
  id: number;
  number: string;
  isLocked: boolean;
  isSelected: boolean;
  isAvailable: boolean;
  orderId: number | null;
}

export interface OrderInfo {
  guests: OrderInfoGuest[];
  tables: OrderInfoTable[];
}

export interface OrderInfoGuest {
  seatId: number;
  seatFullName: string;
}

export interface OrderInfoTable {
  id: number;
  x: number;
  y: number;
  seats: OrderInfoSeat[];
}

export interface OrderInfoSeat {
  id: number;
  isSelected: boolean;
  x: number;
  y: number;
}

export enum OrderType {
  Regular = 0,
  RegularGroup = 10,
  PrivateHire = 20,
  Service = 30
}

export interface OrderExtras {
  menus: OrderMenu[],
  beverages: OrderBeverage[]
}

//Параметры частного бронирования
export class OrderPrivateHire
{
    // Идентификатор автобуса
    busId: number = 0;

    // Дата бронирования
    date: Date = new Date();

    // Бронь на весь день
    isAllDay: boolean = false;

    // Время начала брони
    timeFrom: Time | null = null;

    // Время окончания брони
    timeTo: Time | null = null;

    // Блокировать бронь для черновика
    blockBookingForDraft: boolean = false;

    // Время начала блокировки брони
    blockBookingTimeFrom: Time | null = null;

    // Время окончания блокировки брони
    blockBookingTimeTo: Time | null = null;

    // Точка отправления
    departurePoint: string | null = null;

    // Точка прибытия
    arrivalPoint: string | null = null;

    // Идентификатор маршрута
    routeId: number | null = null;

    // Остановки
    stops: string[] = [];

    // Цена тура
    tourPrice: number = 0;
}

// Результат поиска конфликтов
export class OrdersConflictsResponse
{
    // Заказы
    orders: Order[] = [];

    // Конфликты
    conflicts: OrderConflictsResponse[] = [];    
}

// Конфликт
export class OrderConflictsResponse
{
    // Id заказа
    orderId: number = 0;

    //Id конфликтных мест
    seatIds: number[] | null = [];
    
    // Блокирующий конфликт
    isBlocking: boolean = false;

    // Необходимо одобрение
    needsApprovement: boolean = false;

    // Можно отменить
    canBeCancelled: boolean = false;
}

export enum OrderStepCommand {
    WaitingForPaiment = "WaitingForPaiment",
    Payment = "Payment",
    Cancel = "Cancel",
}

export class CalculationCostTourResponse
{
  // Цена за сиденья
  tourPrice: number = 0;

  // подсчитанный НДС
  vat: number = 0;
  
  // Итоговая цена с НДС
  totalPrice: number = 0;
}

export interface UpgradeOrderRequest
{
  orderId:  Number,
  clickedObject: BusObject
}

export interface ResponseUpgradeTable
{
  id: number,
  price:  number,
  initialPrice:  number,
  isSelected: boolean,
  isAvailable: boolean, 
  isAvailableForUpgrade: boolean,
  seats: ResponseUpgradeSeat[],
}

export interface ResponseUpgradeSeat
{ 
  id: number,
  price:  number,
  initialPrice:  number,
  isSelected: boolean,
  isAvailable: boolean, 
}

export interface SchemeTableModel extends Table {
  id: number;
  number: number;
  isAvailable: boolean;
  x: number;
  y: number;
  xSize: number;
  ySize: number;
  isLeft: boolean;
  isRight: boolean;
  floor: number;
  price: number;
  category: TableCategory;
  initialPrice: number,
  isSelected: boolean,
  isAvailableForUpgrade: boolean,
  seats: SchemeSeatModel[],
}

export interface SchemeSeatModel extends Seat {
  initialPrice: number,
  isSelected: boolean,
  isAvailable: boolean,
}
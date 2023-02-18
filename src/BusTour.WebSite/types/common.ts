import { OrderState, OrderStepCommand, SeatingType } from "./booking";

export interface Dictionary<T> {
    [Key: string]: T;
}

export class SelectItem {
    value: string | number | boolean;
    text: string;
    selected?: boolean = false; 
    label?: string = "";

    constructor(value: any, text?: string | number) 
    { 
        this.value = value.toString();
        this.text = text ? text.toString() : value;
    }

    public static fromEnum(sourceEnum: any, names: any = null) : SelectItem[]
    {
        const result: SelectItem[] = [];

        for(const val in sourceEnum) {
            if (!isNaN(Number(val))) {
                result.push(new SelectItem(Number(val).toString(), (names ? names[sourceEnum[val]] : sourceEnum[val]) as string));
            }
        }

        return result;
    }
}

export class Time {

    hours: number;
    minutes: number;    
    seconds: number;   

    constructor(hours: number, minutes: number, seconds: number = 0) 
    { 
        if (hours < 0 || minutes < 0 || seconds < 0 || hours > 24 || minutes > 60 || seconds > 60) {
            throw true;
        }
        this.hours = hours;
        this.minutes = minutes;
        this.seconds = seconds;
    }    

    public get date(): Date {
        return new Date(2020, 1, 1, this.hours, this.minutes, this.seconds);
    }    

    public static fromDate(date: Date): Time {
        return new Time(date.getHours(), date.getMinutes(), date.getSeconds());
    }         

    public static fromPlain(obj: Time): Time {
        return obj ? new Time(obj.hours, obj.minutes, obj.seconds) : obj;
    }     

    public static fromString(str: string): Time {
        const arr = str.split(':');
        return new Time(Number(arr[0]), Number(arr[1]), Number(arr[2]));
    } 

    public static toString(time: Time): string {
        return `0${time.hours}`.slice(-2) + ':' + `0${time.minutes}`.slice(-2) + ':' +`0${time.seconds}`.slice(-2);
    }  

    public static toDate(time: Time): Date {
        return new Date(2020, 1, 1, time.hours, time.minutes, time.seconds);
    }     
    
    public static diff(from: any, to: any): number {
        return (Time.fromPlain(to)?.date.getTime() ?? 0) - (Time.fromPlain(from)?.date.getTime() ?? 0)
    }

    public toString(): string {
        return Time.toString(this);
    }    

    public toShortString(): string {
        return `0${this.hours}`.slice(-2) + ':' + `0${this.minutes}`.slice(-2);
    }       
    
    public addTime(time: Time): Time {
        let thisDate = new Date(2020, 1, 1, this.hours, this.minutes, this.seconds);
        let timeDate = new Date(thisDate.getTime() + time.hours*3600000 + time.minutes*60000 + time.seconds*1000);
        return new Time(timeDate.getHours(),timeDate.getMinutes(),timeDate.getSeconds());
    }

    public equals(target: Time): boolean {
        return target ? this.hours == target.hours && this.minutes == target.minutes && this.seconds == target.seconds : false;
    }
}

export enum WeekDays
{
    Sun = 0,
    Mon = 1,
    Tue = 2,
    Wed = 3,
    Thu = 4,
    Fri = 5,
    Sat = 6
}

// Класс с методами отображения и преобразования форматов данных (дат и т.д.)
export class DisplayFormatter {
    public static date(date: Date): string {
        return new Intl.DateTimeFormat('en-GB', { 
            month: "long", 
            day: "numeric", 
            year: "numeric"
        }).format(date);
    }

    public static tableType(tableType: SeatingType): string {
        return SeatingType[tableType].match(/[A-Z][^A-Z]*/g)?.join(" ") ?? "";
    }

    public static toDataBaseDateTimeFormat(date: Date) {
        return date.toISOString().substring(0, 19);
    }
}

export interface Action<T> {
    (param: T): void;
}

export interface Func<T, TResult> {
    (param: T): TResult;
}

export interface Predicate {
    (...params: any[]): boolean
}

export enum PaymentMethod
{
    Visa = 1,
    MasterCard = 2, 
    PayPal = 3, 
    ApplePay = 4
}

export type Optional<T> = T | undefined;

export class BaseEntity {
    id: number = 0
}

export interface IDataRequestSort {
    field?: string
    dir?: string
}

export class DataRequestSort implements IDataRequestSort {
    constructor(public field: string = "id", public dir: string = "desc") {}
}

export interface IDataRequestFilter<TFilter> {
    logic?: string
    filters?: TFilter[]
}

export class DataRequestFilter<TFilter> implements IDataRequestFilter<TFilter> {
    constructor(public filters: TFilter[] = [], public logic: string = "and") {}
}

export interface IDataRequest<TFilter> {
    skip?: number
    take?: number
    filter: IDataRequestFilter<TFilter>
    sort?: IDataRequestSort[]
}

export class DataRequest<TFilter> implements IDataRequest<TFilter> {
    public skip?: number;
    public take?: number;
    public filter: DataRequestFilter<TFilter>;
    public sort?: DataRequestSort[];

    constructor(
        filter: TFilter | DataRequestFilter<TFilter>, 
        sort: DataRequestSort[] = [new DataRequestSort()], 
        skip: Optional<number> = undefined, 
        take: Optional<number> = undefined
    ) {
        this.skip = skip;
        this.take = take;
        this.sort = sort;

        if ("filters" in filter) {
            this.filter = filter;
        } else {
            this.filter = new DataRequestFilter<TFilter>([filter]);
        }
    }

    public toString(): string {
        return JSON.stringify(this);
    }
}

export interface IDataResponse<TEntity> {
    items: TEntity[]
    count: number
    aggregates?: object
    groups?: object
}

export interface IBadRequest {
    message?: string
    data?: any
}

export interface IStateMovingCommand<TState> {
    command: TState
    ids: number[]
}

export class OrderStateMovingCommand implements IStateMovingCommand<OrderStepCommand> {
    constructor(public command: OrderStepCommand, public ids: number[]) {}
}
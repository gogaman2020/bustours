import { Payment } from './booking';

export class GiftCertificate {
    id: number | null = null;
    amountVariant: AmountVariant | null = null;
    dateStart: Date | null = null;
    dateEnd: Date | null = null;
    certificateSurprises: CertificateSurprise[] = [];
    comment: string = '';
    number: string | null = null;
    status: GiftCertificateStatus | null = null;
    payment: Payment | null = null;
}

export class GiftCertificateAdding {
    amountVariantId: number = 1;
    hasSurprises: boolean = false;
    certificateSurprises: CertificateSurprise[] = []; 
    comment: string = '';
    addedCertificate: GiftCertificate = new GiftCertificate();
    id: number  = 0;
}

export interface AmountVariant {
    id: number;
    amount: number;
}

export interface CertificateSurprise {
    id: number;
    certificateId: number;
    surpriseId: number;
    quantity: number;
}

export interface CertificateFilter {
    number: string | null;
    statuses: GiftCertificateStatus[];
}

export interface GiftCertificateStatusTotals {
    status: number;
    amount: number;
    count: number;
    balance: number;
}

export enum GiftCertificateStatus
{
    Draft = 0,
    Active = 10,
    Expired = 20,
    Redeemed = 30,
    Сancelled = 99,
}
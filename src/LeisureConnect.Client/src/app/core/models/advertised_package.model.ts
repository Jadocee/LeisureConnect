import { ServiceItemSummary } from "./service_item_summary.model";

export interface AdvertisedPackage {
    packageId: number;
    name: string;
    description: string;
    startDate: string;
    endDate: string;
    advertisedPrice: number;
    currencyCode: string;
    inclusions?: string;
    exclusions?: string;
    gracePeriodDays: number;
    isStandardPackage: boolean;
    includedServices: ServiceItemSummary[];
}
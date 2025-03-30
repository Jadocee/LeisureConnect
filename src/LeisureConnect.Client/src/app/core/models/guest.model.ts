export interface Guest {
    firstName: string;
    lastName: string;
    email?: string;
    phoneNumber?: string;
    address?: string;
    isPrimaryGuest: boolean;
}
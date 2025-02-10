import { RegisterPhoneModel } from "./register-phone-model";
import { Role } from "./role.enum";

export class RegisterModel {
    email: string | undefined;
    firstName: string | undefined;
    lastName: string | undefined;
    docNumber: string | undefined;
    docType: number | undefined;
    phones: RegisterPhoneModel[] = [];
    birthDate: string | undefined;
    managerId?: string;
    password: string | undefined;
    role: Role | undefined;
}

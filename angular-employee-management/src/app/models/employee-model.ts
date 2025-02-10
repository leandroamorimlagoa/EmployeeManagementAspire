import { RegisterPhoneModel } from "./register-phone-model";
import { Role } from "./role.enum";

export class EmployeeModel {
    id: string = '';
    email: string = '';
    firstName: string = '';
    lastName: string = '';
    docNumber: string = '';
    docType: number = 0;
    phones: RegisterPhoneModel[] = [];
    birthDate: string = '';
    managerId?: string;
    password: string = '';
    role: Role = Role.Employee;
  
    constructor(init?: Partial<EmployeeModel>) {
      Object.assign(this, init);
    }
}

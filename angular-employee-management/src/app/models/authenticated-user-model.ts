import { Role } from "./role.enum";

export class AuthenticatedUserModel {
    id: string | undefined;
    email: string | undefined;
    firstName: string | undefined;
    lastName: string | undefined;
    role?: Role | undefined;
}

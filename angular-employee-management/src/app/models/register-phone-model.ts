export class RegisterPhoneModel {
    phoneNumber: string = '';
  
    constructor(init?: Partial<RegisterPhoneModel>) {
      Object.assign(this, init);
    }
}

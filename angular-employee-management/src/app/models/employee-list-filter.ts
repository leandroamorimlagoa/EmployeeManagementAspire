export class EmployeeListFilter {
    searchTerm: string = '';
    currentPage: number = 1;
    pageSize: number = 10;
    totalPages: number = 1;
    pages: number[] = [];
}

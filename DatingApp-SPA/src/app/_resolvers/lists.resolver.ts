import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ListsResolver implements Resolve<User[]> {
    pageNumber = 1;
    pageSize = 12;
    likesParam = 'Likers';

    constructor(private userService: UserService, private router: Router, private toastr: ToastrService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likesParam).pipe(
            catchError(error => {
                this.toastr.toastrConfig.preventDuplicates = true;
                this.toastr.error('Could not retrieve Lists data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}

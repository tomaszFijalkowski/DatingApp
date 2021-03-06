import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberDetailResolver implements Resolve<User> {
    constructor(private userService: UserService, private router: Router,
        private authService: AuthService, private toastr: ToastrService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        if (this.authService.currentUser.id === +route.params.id) {
            this.router.navigate(['/member/edit']);
        }

        return this.userService.getUser(route.params['id']).pipe(
            catchError(error => {
                this.toastr.toastrConfig.preventDuplicates = true;
                this.toastr.error('Could not retrieve User data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
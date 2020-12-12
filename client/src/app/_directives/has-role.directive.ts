import { AccountService } from './../_services/account.service';
import { Directive, ViewContainerRef, TemplateRef, Input, OnInit } from '@angular/core';
import { map, take } from 'rxjs/operators';
import { User } from '../_models/user';

@Directive({
  selector: '[appHasRole]', // *appHasRole='["Admin"]' -- when we use this directive
})
export class HasRoleDirective implements OnInit{
  @Input() appHasRole: string[]; // this. is the array of roles that the user need to be part of in order to view the template
  user: User;
  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService)
   {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    });
  }
  ngOnInit(): void {
    // clear the view if no roles
    if(!this.user?.roles || this.user == null) {
      this.viewContainerRef.clear();
      return;
    }

    if(this.user?.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }
}

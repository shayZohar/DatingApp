import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { AdminService } from './../../_services/admin.service';
import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe( users => {
      this.users = users;
    });
  }

  // activating the modal component
  openRolesModal(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent,config);// showing the modal with given initial data
    this.bsModalRef.content.updateSelectedRoles.subscribe(values => { // subscribing to the modal input
      const rolesToUpdate = {
        roles: [...values.filter(el => el.checked === true).map(el => el.name)] // checking for changes and updating
      };
      if(rolesToUpdate) {
        this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(()=>{
          user.roles = [...rolesToUpdate.roles];
        })
      }
    });

  }

  private getRolesArray(user: User) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] =
     [
      {name: 'Admin', value:'Admin'},
      {name: 'Moderator', value:'Moderator'},
      {name: 'Member', value:'Member'},
     ];

     availableRoles.forEach(role => {
       let isMatch = false;
       for(const userRole of userRoles)
       {
         if(role.name === userRole) {
           isMatch = true;
           role.checked = true;
           roles.push(role);
           break;
         };
       };

       if(!isMatch) {
         role.checked = false;
         roles.push(role);
       }
     })
     return roles;
  }

}

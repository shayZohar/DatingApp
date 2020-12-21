import { Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'presence',
    {
     accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build();

    // starting the hub service
    this.hubConnection.start().catch(error => console.log(error));

    // listening to new connection of other users, the info comes from the api 'PresenceHub'
    this.hubConnection.on('UserIsOnline',username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames,username])
      })
    });

    this.hubConnection.on('UserIsOffline',username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames.filter(x => x !== username)])
      })
    });

    this.hubConnection.on('GetOnlineUsers',(usernames: string[]) => {
      this.onlineUsersSource.next(usernames); // updating the observable with current online user list
    });

    this.hubConnection.on('NewMessageReceived',({username,knownAs}) => {
      this.toastr.info(knownAs + ' has sent you a new message!')
      .onTap
      .pipe(take(1)).subscribe(()=> this.router.navigateByUrl('/members/' +username + '?tab=3')); // take the user to the tab of messages of sender user
    });
  }

  stopHubConnection() {
    this.hubConnection.stop().catch(error => console.log(error));
}
}

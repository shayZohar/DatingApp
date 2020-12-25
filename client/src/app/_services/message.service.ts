import { BusyService } from './busy.service';
import { Group } from './../_models/group';
import { BehaviorSubject } from 'rxjs';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Message } from 'src/app/_models/message';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { User } from '../_models/user';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private MessageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.MessageThreadSource.asObservable();

  constructor(private http: HttpClient, private busyService: BusyService) {}

  createHubConnection(user: User, otherUsername: string) {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl +'message?user=' + otherUsername, {
      accessTokenFactory: () => user.token
    }).withAutomaticReconnect().build();

    this.hubConnection.start()
    .catch(error => console.log(error)).finally(() => this.busyService.idle());

    this.hubConnection.on('ReceiveMessageThread',messages => {
      this.MessageThreadSource.next(messages); // getting the messages to the observable
    });

    //adding new message to the observable array of messages
    this.hubConnection.on('NewMessage',message => {
      this.messageThread$.pipe(take(1)).subscribe(messages => [
        this.MessageThreadSource.next([...messages,message]) // creating new array
      ])
    });

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if(group.connections.some(x => x.username === otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if(!message.dateRead) {
              message.dateRead = new Date(Date.now());
            }
          });
          this.MessageThreadSource.next([...messages]);
        })
      }
    })
  }

  stopHubConnection() {
    if(this.hubConnection) {
      this.MessageThreadSource.next([]); // clearing the messages when disconnecting so if other user get in it wont be shown for 1 second
      this.hubConnection.stop();
    }
  }
  getMessages(pageNumber, pageSize, container) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  async sendMessage(username: string, content: string) {
    // this returns a promise
    return this.hubConnection.invoke('SendMessage', {recipientUsername: username, content})
      .catch(error => console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}

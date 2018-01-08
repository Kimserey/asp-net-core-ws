import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs/Rx';
import { WebsocketService } from './websocket';
import { Message } from './model';
import { environment } from '../environments/environment';

@Injectable()
export class MessageService {
  public messages$: Observable<Message>;

  constructor(private webSocketService: WebsocketService, private http: HttpClient) { }

  connect(username) {
    this.messages$ = this.webSocketService
      .connect(username, environment.ws)
      .map(ev => ev.data);
  }

  send(username, content) {
    this.http
      .post(`${environment.api}/messages/${username}`, { content: content })
      .subscribe(data => { });
  }
}

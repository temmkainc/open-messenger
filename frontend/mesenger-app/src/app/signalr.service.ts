import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;

  startConnection(userId: number): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/chathub`, {accessTokenFactory: () => localStorage.getItem('accessToken') || ''})
      .withAutomaticReconnect()
      .build();
  
    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connected!');
        // When connection starts, join the first conversation or current one
        this.hubConnection.invoke('JoinConversation', userId)
          .catch(err => console.error('JoinConversation failed:', err));
      })
      .catch(err => console.error('SignalR connection error: ', err));
  }

  joinConversation(conversationId: number) {
    this.hubConnection.invoke('JoinConversation', conversationId)
      .catch(err => console.error('Error joining conversation', err));
  }

  onMessageReceived(callback: (message: any) => void): void {
    this.hubConnection.on('ReceiveMessage', (messageDto: any) => {
      console.log('Received message from SIGNALR', messageDto);
      callback(messageDto);
    });
  }

}

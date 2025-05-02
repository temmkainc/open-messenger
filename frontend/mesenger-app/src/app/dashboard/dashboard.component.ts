import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { SignalrService } from '../signalr.service';
import { FormsModule } from '@angular/forms';
import { ViewChild, ElementRef } from '@angular/core';
import { NgZone } from '@angular/core';



@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {


  @ViewChild('messagesContainer') messagesContainer!: ElementRef;


  user: any = null;
  isLoggedIn: boolean = false;
  private apiUrl = `${environment.apiUrl}/User`; 

  constructor(
    private authService: AuthService,
    private router: Router,
    private http: HttpClient,
    private signalRService : SignalrService,
    private ngZone: NgZone
  ) {}

  selectedConversation: any = null;
  messages: any[] = [];
  newMessage: string = '';

  searchQuery: string = '';
  searchResults: any[] = [];

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile() {
    this.http.get<any>(`${this.apiUrl}/profile`)
    .subscribe({
      next: (user) => {
        this.user = user;
        this.isLoggedIn = true;


        this.loadUserConversations();
        this.signalRService.startConnection(this.user.id);
        this.signalRService.onMessageReceived((messageDto) => {
          if(messageDto.senderId === this.user.id) return;
          this.messages.push(messageDto); 
          setTimeout(() => this.scrollToBottom(), 5);
        });



      },
      error: (err) => {
        console.error('Profile load failed:', err);
        this.isLoggedIn = false;
        this.authService.logoutUserLocally();
        this.router.navigate(['/login']);
      }
    });
  }

  onLogout() {
    const userId = this.authService.getUserId();
    if (!userId) {
      console.error('No userId found in storage. Logging out locally.');
      this.authService.logoutUserLocally();
      this.router.navigate(['/login']);
      return;
    }

    const logoutDto = { userId: this.user.id};

    this.authService.logout(logoutDto).subscribe({
      next: (response) => {
        console.log('Logout successful:', response);
        this.authService.logoutUserLocally();
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('Logout failed:', error);
        this.authService.logoutUserLocally();
        this.router.navigate(['/login']);
      }
    });
  }

  conversations: any[] = [];

  loadUserConversations() {
    const userId = this.user.id;
    this.http.get<any[]>(`${environment.apiUrl}/Conversation/mine`)
      .subscribe({
        next: (data) => {
          this.conversations = data;
          console.log('Conversations: ', this.conversations);
          this.conversations.forEach(convo => {
            this.signalRService.joinConversation(convo.id);
          });
        },
        error: (err) => {
          console.error('Failed to load conversations', err);
        }
      });
  }
  getOtherParticipantName(convo: any): string {
    if (!convo || !Array.isArray(convo.participants)) {
      return 'Unknown';
    }
  
    const other = convo.participants.find((p: any) => p.id !== this.user?.id);
    return other?.username || 'Unknown';
  }

  selectConversation(convo: any) {
    this.selectedConversation = convo;
    this.loadMessages(convo.id);
    this.signalRService.joinConversation(convo.id);
  }

  loadMessages(convoId: number) {
    this.http.get<any[]>(`${environment.apiUrl}/Message/${convoId}`)
      .subscribe({
        next: (data) => {
          this.messages = data;
          setTimeout(() => this.scrollToBottom(), 5);
        },
        error: (err) => {
          console.error('Failed to load messages:', err);
        }
      });
  }

  sendMessage() {
    if (!this.newMessage.trim()) return;
  
    const convoId = this.selectedConversation.id;
    const message = {
      senderId: localStorage.getItem('userId'),
      text: this.newMessage,
      senderUsername: this.user.username
    };

  
    this.http.post(`${environment.apiUrl}/Message/${convoId}`, message, {
      headers: { 'Content-Type': 'application/json' }
    }).subscribe({
      next: (msg: any) => {
        if (!msg.senderUsername) {
          msg.senderUsername = this.user.username; 
        }
        this.messages.push(msg);
        this.newMessage = '';
        setTimeout(() => this.scrollToBottom(), 5);
      },
      error: (err) => {
        console.error('Failed to send message via API:', err);
      }
    });
  }
  
  onSearchChange() {
    if (this.searchQuery.trim().length < 2) {
      this.searchResults = [];
      return;
    }
  
    this.http
      .get<any[]>(`${environment.apiUrl}/User/search?query=${this.searchQuery}`)
      .subscribe({
        next: (users) => {
          const alreadyInConversationIds = this.conversations
            .flatMap(c => c.participants.map((p: { id: any; }) => p.id));
          this.searchResults = users.filter(u => 
            u.id !== this.user.id && !alreadyInConversationIds.includes(u.id));
        },
        error: (err) => console.error('Search failed', err)
      });
  }

  startConversationWith(user: any) {
    const newConversation = {
      participantIds: [user.id]
    };
  
    this.http
      .post<any>(`${environment.apiUrl}/Conversation`, newConversation)
      .subscribe({
        next: (conversation) => {
          this.conversations.push(conversation);
          this.selectConversation(conversation);
          this.searchQuery = '';
          this.searchResults = [];
        },
        error: (err) => console.error('Failed to start conversation:', err)
      });
  }

  scrollToBottom(): void {
    if (this.messagesContainer && this.messagesContainer.nativeElement) {
      try {
        this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
      } catch (err) {
        console.error('Failed to scroll', err);
      }
    }
  }
  

  ngAfterViewInit() {
    setTimeout(() => this.scrollToBottom(), 0);
  }
}

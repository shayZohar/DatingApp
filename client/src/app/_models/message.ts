export interface Message {
  id: number;
  senderId: number;
  senderUsername: string;
  senderphotoUrl: string;
  recipentId: number;
  recipentUsername: string;
  recipentPhotoUrl: string;
  content: string;
  dateRead?: Date;
  messageSent: Date;
}

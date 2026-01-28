export interface CambiarPasswordRequest {
  newPassword: string;
  confirmNewPassword: string;
  token: string;
  userId: number;
  idModule: number;
}

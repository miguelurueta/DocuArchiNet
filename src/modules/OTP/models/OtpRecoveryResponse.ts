export interface OtpRecoveryResponse {
  token: string;
  expiracion: string;
  usuario: {
    usuarioId: number;
    login: string;
    email: string;
  };
}

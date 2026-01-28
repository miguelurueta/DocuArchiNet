export interface RecuperarPasswordResponse {
  challengeId: string;
  destinoEnmascarado: string;
  tiempoExpira: number; // en minutos
  userId: number;
  idModule: number;
}

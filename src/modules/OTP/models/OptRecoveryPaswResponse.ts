export interface OptRecoveryPaswResponse {
  token: string;
  expiracion: string;
  idModule:number;
  usuario: {
  usuarioId: number;
  login: string;
  email?: string;              // opcional
  nombre: string;
  activo: boolean;
  fechaLimiteAcceso?: string;  // en JSON suele venir como string (ISO date)
  permisos: string[];
  };
  
}
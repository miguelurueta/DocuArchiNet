import type Claim from "../../../app/auth/Dto/Claim";

export default interface UsuarioAutenticado {
  usuarioId: number;
  login: string;
  email?: string;
  nombre: string;
  activo: boolean;
  fechaLimiteAcceso?: Date;
  permisos: string[];
  claims:Claim[];
}

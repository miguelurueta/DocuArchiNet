import type UsuarioAutenticado from "./UsuarioAutenticado.model";
import type Claim from "../../../app/auth/Dto/Claim";
export default interface RespuestaAutenticacion{
    token: string;
    expiracion: Date | string;
    usuario: UsuarioAutenticado;
    claims?: Claim[];
} 

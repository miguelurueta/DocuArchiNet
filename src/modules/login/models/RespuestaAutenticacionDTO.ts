import type UsuarioAutenticado from "./UsuarioAutenticado.model";
export default interface RespuestaAutenticacion{
    token: string;
    expiracion: Date;
    usuario: UsuarioAutenticado;
} 
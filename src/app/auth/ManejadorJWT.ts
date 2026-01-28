import type RespuestaAutenticacion from "../../modules/login/models/RespuestaAutenticacionDTO";
import type Claim from "./Claim";



const llaveToken = "token";
const llaveExpiracion = "token-expiracion";

export function usuarioEstaLogueado(){
    const claims = obtenerClaims();
    return claims.length > 0;
}

export function guardarTokenLocalStorage(autenticacion: RespuestaAutenticacion){
    localStorage.setItem(llaveToken, autenticacion.token); // 👈 corregido
    localStorage.setItem(llaveExpiracion, autenticacion.expiracion.toString());
}

export function logout(){
    localStorage.removeItem(llaveToken);
    localStorage.removeItem(llaveExpiracion);
}

export function obtenerClaims(): Claim[] {
    const token = localStorage.getItem(llaveToken);
    const expiracion = localStorage.getItem(llaveExpiracion);

    if (!token || !expiracion){
        return [];
    }

    const expiracionFecha = new Date(expiracion);

    if (isNaN(expiracionFecha.getTime()) || expiracionFecha <= new Date()){
        logout();
        return [];
    }

    try {
        const payloadBase64 = token.split('.')[1];
        const payloadJson = atob(payloadBase64);
        const dataToken = JSON.parse(payloadJson);

        const claims: Claim[] = Object.entries(dataToken)
                                .map(([nombre, valor]) => ({nombre, valor: String(valor)}));

        return claims;
    }
    catch(err){
        console.error(err);
        logout();
        return [];
    }
 
}

export function obtenerToken(){
    return localStorage.getItem(llaveToken);
}
export function tokenExpirado(): boolean {
  const token = localStorage.getItem(llaveToken);
  const expiracion = localStorage.getItem(llaveExpiracion);

  if (!token || !expiracion) return true;

  const expiracionFecha = new Date(expiracion);
  if (isNaN(expiracionFecha.getTime())) return true;

  return expiracionFecha <= new Date();
}
export function finalizarSesionYRedirigir() {
  logout();
  window.location.href = "/LoginPage";
}

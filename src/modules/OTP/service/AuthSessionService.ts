import { guardarTokenLocalStorage } from "../../../app/auth/Infraestructura/ManejadorJWT";
import type RespuestaAutenticacion from "../../login/models/RespuestaAutenticacionDTO";


export class AuthSessionService {
  static iniciarSesion(auth: RespuestaAutenticacion) {
    guardarTokenLocalStorage(auth);
  }

  static cerrarSesion() {
    localStorage.clear(); // o helper dedicado
  }
}

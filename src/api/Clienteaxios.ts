import axios from "axios";
import { finalizarSesionYRedirigir, obtenerToken, tokenExpirado } from "../app/auth/Infraestructura/ManejadorJWT";


const clienteApi = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});
clienteApi.interceptors.request.use((config) => {
  
  const token = obtenerToken();
  //console.log("TOKEN USADO POR AXIOS:", token);
  if (token) {
    // ✅ 1) Si está expirado, cerramos sesión y cancelamos el request
    if (tokenExpirado()) {
      finalizarSesionYRedirigir();
      return Promise.reject(new Error("TOKEN_EXPIRADO"));
    }
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
// 🔑 INTERCEPTOR DE RESPUESTA
clienteApi.interceptors.response.use(
  response => response,
  error => {
    if (import.meta.env.MODE !== "production") {
      // 🧨 ERROR COMPLETO DE AXIOS
      console.group("❌ AXIOS ERROR");

      console.error("➡️ Mensaje:", error.message);
      console.error("➡️ Código:", error.code);
      console.error("➡️ Config:", error.config);

      if (error.response) {
        console.error("➡️ Status:", error.response.status);
        console.error("➡️ Status Text:", error.response.statusText);
        console.error("➡️ Data:", error.response.data);
        console.error("➡️ Headers:", error.response.headers);
      } else if (error.request) {
        console.error("➡️ Request (sin respuesta):", error.request);
      } else {
        console.error("➡️ Error desconocido:", error);
      }

      console.groupEnd();
    }
  
    // 🔁 IMPORTANTE: seguir propagando el error
    return Promise.reject(error);
  }
);

export default clienteApi;

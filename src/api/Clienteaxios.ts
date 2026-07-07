import axios from "axios";
import { finalizarSesionYRedirigir, obtenerToken, tokenExpirado } from "../app/auth/Infraestructura/ManejadorJWT";

const clienteApi = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  withCredentials: true,
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
  (response) => response,
  (error) => {
    // Cancelaciones por concurrencia (AbortController / requests stale) no son errores funcionales.
    // Evitar ensuciar la consola y confundir diagnósticos.
    if (error?.code === "ERR_CANCELED") {
      return Promise.reject(error);
    }

    if (import.meta.env.MODE !== "production") {
      const status = error?.response?.status;
      const isWarningStatus = status === 400 || status === 401 || status === 409;
      const groupFn = isWarningStatus ? console.groupCollapsed : console.group;
      const logFn = isWarningStatus ? console.warn : console.error;
      const groupLabel = isWarningStatus ? "⚠️ AXIOS WARNING" : "❌ AXIOS ERROR";

      groupFn(groupLabel);
      logFn("➡️ Mensaje:", error.message);
      logFn("➡️ Código:", error.code);
      logFn("➡️ Config:", error.config);

      if (error.response) {
        logFn("➡️ Status:", error.response.status);
        logFn("➡️ Status Text:", error.response.statusText);
        logFn("➡️ Data:", error.response.data);
        logFn("➡️ Headers:", error.response.headers);
      } else if (error.request) {
        logFn("➡️ Request (sin respuesta):", error.request);
      } else {
        logFn("➡️ Error desconocido:", error);
      }

      console.groupEnd();
    }

    // 🔁 IMPORTANTE: seguir propagando el error
    return Promise.reject(error);
  },
);

export default clienteApi;


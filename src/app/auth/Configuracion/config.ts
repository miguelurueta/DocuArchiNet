export type TokenStrategy = "redirect" | "renew";
export const authConfig = {
  tokenStrategy: "redirect" as TokenStrategy, // o "renew"
  checkIntervalMs: 30_000, // 30 segundos
  avisoDelayMs: 5_000, // tiempo antes de redirigir
};

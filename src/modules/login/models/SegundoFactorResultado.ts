export default interface SegundoFactorResultado {
  requiereSegundoFactor: boolean;
  challengeId: string;
  proveedor: "EMAIL" | "TOTP";
  destinoEnmascarado?: string;
  tiempoExpira: number;
}

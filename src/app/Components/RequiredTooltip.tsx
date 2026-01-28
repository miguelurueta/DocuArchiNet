type RequiredTooltipProps = {
  /**
   * 🔁 MODO LEGACY
   * Usado por los componentes actuales
   * NO se rompe nada existente
   */
  visible?: boolean;

  /**
   * 🆕 MODO FIELD-DRIVEN
   */
  field?: string;
  invalidField?: string | null;

  /**
   * 🆕 Texto personalizado del tooltip
   */
  message?: string;
};

export default function RequiredTooltip({
  visible,
  field,
  invalidField,
  message,
}: RequiredTooltipProps) {

  const text = message ?? "Este campo es obligatorio.";

  // ============================
  // 🔁 COMPATIBILIDAD LEGACY
  // ============================
  if (typeof visible === "boolean") {
    if (!visible) return null;
    return (
      <div className="tooltip-required">
        {text}
      </div>
    );
  }

  // ============================
  // 🆕 MODO FIELD-DRIVEN
  // ============================
  if (!field || field !== invalidField) return null;

  return (
    <div className="tooltip-required">
      {text}
    </div>
  );
}

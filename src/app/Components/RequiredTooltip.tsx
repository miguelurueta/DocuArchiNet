type RequiredTooltipProps = {
  /**
   * 🔁 MODO LEGACY
   * Usado por los componentes actuales
   * NO se rompe nada existente
   */
  visible?: boolean;

  inline?: boolean;

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
  inline = false,
  field,
  invalidField,
  message,
}: RequiredTooltipProps) {

  const text = message ?? "Este campo es obligatorio.";
  const className = inline ? "tooltip-required tooltip-required-inline" : "tooltip-required";

  // ============================
  // 🔁 COMPATIBILIDAD LEGACY
  // ============================
  if (typeof visible === "boolean") {
    if (!visible) return null;
    return (
      <div className={className}>
        {text}
      </div>
    );
  }

  // ============================
  // 🆕 MODO FIELD-DRIVEN
  // ============================
  if (!field || field !== invalidField) return null;

  return (
    <div className={className}>
      {text}
    </div>
  );
}

import { useState, useRef, useEffect } from "react";

export interface SelectorOption {
  value: number;
  label: string;
}

interface SelectorModulosProps {
  value: number;
  options: SelectorOption[];
  onChange: (value: number) => void;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
}

export default function SelectorModulos({
  value,
  options,
  onChange,
  placeholder = "Seleccione un módulo",
  required = false,
  disabled = false,
}: SelectorModulosProps) {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  const selected = options.find(o => o.value === value);

  // Cerrar al hacer click fuera (comportamiento legacy)
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <div
      ref={ref}
      className={`custom-select ${disabled ? "disabled" : ""}`}
      data-required={required ? "true" : undefined}
    >
      <div
        className="selected"
        data-value={value || ""}
        onClick={() => !disabled && setOpen(o => !o)}
      >
        <span>{selected?.label ?? placeholder}</span>
        <i className="fa-solid fa-circle-chevron-down" />
      </div>

      {open && !disabled && (
        <ul className="options">
          {options.map(opt => (
            <li
              key={opt.value}
              data-value={opt.value}
              onClick={() => {
                onChange(opt.value);
                setOpen(false);
              }}
            >
              {opt.label}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

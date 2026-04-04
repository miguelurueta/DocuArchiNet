import { useEffect, useMemo, useState } from "react";
import { AutoComplete, Card, Col, Form, Row, Select, Space, Tooltip } from "antd";
import { InfoCircleOutlined } from "@ant-design/icons";
import type { FocusEventHandler } from "react";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import { useAutocompleteCamposPlantilla } from "../hooks/useAutocompleteCamposPlantilla";
import styles from "../style/FormRadicacion.module.css";

/**
 * Extensión:
 * - Para cambiar el endpoint o parámetros, ajuste useAutocompleteCamposPlantilla (hook).
 * - Para nuevas validaciones o formatos, transforme las opciones en mapOptions.
 * - Para nuevos tipos de campo, agregue una variante en CampoPlantillaAutoCompleteField.
 */

type CampoPlantillaEx = CampoPlantillaDTO & {
  data_group?: string | null;
};

const resolveCampoIdScript = (campo: CampoPlantillaEx): number | undefined => {
  const nestedId = campo.TomPParameterTomSelelect?.id_escript;
  if (typeof nestedId === "number" && Number.isFinite(nestedId)) {
    return nestedId;
  }
  const anyCampo = campo as unknown as { id_escript?: number | null };
  if (typeof anyCampo.id_escript === "number" && Number.isFinite(anyCampo.id_escript)) {
    return anyCampo.id_escript;
  }
  return undefined;
};

interface CamposPlantillaAutoCompleteRendererProps {
  camposPlantilla: ReadonlyArray<CampoPlantillaEx>;
  className?: string;
  fieldClassName?: (field: CampoPlantillaEx) => string | undefined;
  value?: Record<string, string>;
  defaultValue?: Record<string, string>;
  onChange?: (value: string, field: CampoPlantillaEx) => void;
  onBlur?: FocusEventHandler<HTMLInputElement | HTMLSelectElement>;
  onFocus?: FocusEventHandler<HTMLInputElement | HTMLSelectElement>;
  translate?: (value: string, field: CampoPlantillaEx) => string;
}

function useDebouncedValue(value: string, delayMs: number) {
  const [debounced, setDebounced] = useState(value);

  useEffect(() => {
    const handle = setTimeout(() => setDebounced(value), delayMs);
    return () => clearTimeout(handle);
  }, [delayMs, value]);

  return debounced;
}

function getLabelText(
  field: CampoPlantillaEx,
  translate?: (value: string, field: CampoPlantillaEx) => string,
) {
  const raw = field.aleas_campo ?? field.name_campo;
  const resolved = translate ? translate(raw, field) : raw;
  const lower = resolved.toLocaleLowerCase();
  return lower.replace(/(^|\s)(\S)/g, (_fullMatch, prefix, letter) => {
    return `${prefix}${letter.toLocaleUpperCase()}`;
  });
}

function getTooltipText(
  field: CampoPlantillaEx,
  translate?: (value: string, field: CampoPlantillaEx) => string,
) {
  const raw = field.tooltipAyuda ?? "";
  return raw ? (translate ? translate(raw, field) : raw) : "";
}

function getTitleText(
  field: CampoPlantillaEx,
  translate?: (value: string, field: CampoPlantillaEx) => string,
) {
  const raw = field.title_control ?? "";
  return raw ? (translate ? translate(raw, field) : raw) : undefined;
}

function getErrorMessage(error: unknown) {
  if (!error) {
    return "";
  }
  return "No fue posible cargar las opciones. Intenta nuevamente.";
}

function mapOptions(items: ReadonlyArray<{ idValue: string | null; texValue: string }>) {
  return items.map((item, index) => ({
    value: item.texValue ?? "",
    label: item.texValue ?? `Opción ${index + 1}`,
  }));
}

function normalizeDynamicFieldValue(value: unknown) {
  if (typeof value === "string") {
    return value;
  }
  if (typeof value === "number" || typeof value === "boolean") {
    return String(value);
  }
  return "";
}

function joinClassNames(...values: Array<string | undefined>) {
  return values.filter(Boolean).join(" ");
}

export function CampoPlantillaAutoCompleteField({
  campo,
  value,
  defaultValue,
  className,
  onChange,
  onBlur,
  onFocus,
  translate,
}: {
  campo: CampoPlantillaEx;
  value?: string;
  defaultValue?: string;
  className?: string;
  onChange?: (value: string, field: CampoPlantillaEx) => void;
  onBlur?: FocusEventHandler<HTMLInputElement | HTMLSelectElement>;
  onFocus?: FocusEventHandler<HTMLInputElement | HTMLSelectElement>;
  translate?: (value: string, field: CampoPlantillaEx) => string;
}) {
  const [inputValue, setInputValue] = useState(
    normalizeDynamicFieldValue(value ?? defaultValue),
  );
  const debouncedValue = useDebouncedValue(inputValue, 300);

  const nameCampo = campo.name_campo;
  const dataIdent = `pl-radicacion-spe-${nameCampo}`;
  const tblControl = campo.tbl_control ?? "";
  const campoIdScript = resolveCampoIdScript(campo);
  const labelText = getLabelText(campo, translate);
  const tooltipText = getTooltipText(campo, translate);
  const titleText = getTitleText(campo, translate);

  const shouldQuery = debouncedValue.trim().length > 0;

  const { data, error } = useAutocompleteCamposPlantilla(
    shouldQuery
      ? {
          TextoBuscado: debouncedValue,
          defaultDbAlias: "",
          tbl_control: tblControl,
          name_campo: nameCampo,
          ...(campoIdScript !== undefined ? { idScript: campoIdScript } : {}),
        }
      : null,
    shouldQuery,
  );

  const options = useMemo(() => mapOptions(data), [data]);
  const errorMessage = getErrorMessage(error);
  const normalizedControlledValue =
    value !== undefined ? normalizeDynamicFieldValue(value) : undefined;
  const resolvedValue = normalizedControlledValue ?? inputValue;

  useEffect(() => {
    if (normalizedControlledValue !== undefined) {
      setInputValue(normalizedControlledValue);
    }
  }, [normalizedControlledValue]);

  const tooltipId = tooltipText
    ? `pl-radicacion-spe-tooltip-${nameCampo}`
    : undefined;

  const labelNode = (
    <span title={titleText} className={styles.labelCapitalize}>
      {labelText}
      {tooltipText ? (
        <Tooltip title={tooltipText}>
          <span
            className={`${styles["tooltip-ayuda"]} tooltip-ayuda`.trim()}
            role="button"
            tabIndex={0}
            aria-label={`Mostrar ayuda para ${labelText}`}
            aria-describedby={tooltipId}
            data-tooltip-id={tooltipId}
          >
            <InfoCircleOutlined />
          </span>
        </Tooltip>
      ) : null}
    </span>
  );

  return (
    <Form.Item
      label={labelNode}
      required={campo.obligatorio_campo === 1}
      validateStatus={errorMessage ? "error" : undefined}
      help={errorMessage || undefined}
      data-ident={dataIdent}
    >
      <AutoComplete
        className={joinClassNames(styles.dynamicAutocomplete, className)}
        value={resolvedValue}
        options={options}
        onSearch={(val) => {
          setInputValue(normalizeDynamicFieldValue(val));
        }}
        onChange={(val) => {
          const normalized = normalizeDynamicFieldValue(val);
          setInputValue(normalized);
          onChange?.(normalized, campo);
        }}
        onBlur={onBlur}
        onFocus={onFocus}
        placeholder={campo.placeholder ?? undefined}
        disabled={campo.disable_campo === 1}
        data-ident={dataIdent}
        aria-label={labelText}
        aria-describedby={tooltipId}
      />
    </Form.Item>
  );
}

function SelectField({
  campo,
  value,
  defaultValue,
  className,
  onChange,
  onBlur,
  onFocus,
  translate,
}: {
  campo: CampoPlantillaEx;
  value?: string;
  defaultValue?: string;
  className?: string;
  onChange?: (value: string, field: CampoPlantillaEx) => void;
  onBlur?: FocusEventHandler<HTMLInputElement | HTMLSelectElement>;
  onFocus?: FocusEventHandler<HTMLInputElement | HTMLSelectElement>;
  translate?: (value: string, field: CampoPlantillaEx) => string;
}) {
  const nameCampo = campo.name_campo;
  const dataIdent = `pl-radicacion-spe-${nameCampo}`;
  const labelText = getLabelText(campo, translate);
  const tooltipText = getTooltipText(campo, translate);
  const titleText = getTitleText(campo, translate);

  const tooltipId = tooltipText
    ? `pl-radicacion-spe-tooltip-${nameCampo}`
    : undefined;

  const labelNode = (
    <span title={titleText} className={styles.labelCapitalize}>
      {labelText}
      {tooltipText ? (
        <Tooltip title={tooltipText}>
          <span
            className={`${styles["tooltip-ayuda"]} tooltip-ayuda`.trim()}
            role="button"
            tabIndex={0}
            aria-label={`Mostrar ayuda para ${labelText}`}
            aria-describedby={tooltipId}
            data-tooltip-id={tooltipId}
          >
            <InfoCircleOutlined />
          </span>
        </Tooltip>
      ) : null}
    </span>
  );

  const resolvedValue = value ?? undefined;
  const resolvedDefaultValue = value === undefined ? defaultValue : undefined;
  const selectClassName = [styles.dynamicSelect, className].filter(Boolean).join(" ");
  const options = campo.ilist_row_drowlist ?? [];
  const parsedOptions = options.map((option, index) => {
    const anyOption = option as unknown as {
      idValue?: string | number | null;
      Value?: string | null;
      id_value?: string | number | null;
      value_campo?: string | null;
    };
    const optionValue = anyOption.idValue ?? anyOption.id_value ?? String(index);
    const optionLabel =
      anyOption.Value ?? anyOption.value_campo ?? String(optionValue ?? "");
    return {
      value: optionValue ?? "",
      label: optionLabel,
    };
  });

  return (
    <Form.Item
      label={labelNode}
      required={campo.obligatorio_campo === 1}
      data-ident={dataIdent}
      data-api-method={campo.apiMethod ?? undefined}
    >
      <Select
        className={selectClassName}
        id={dataIdent}
        value={resolvedValue}
        defaultValue={resolvedDefaultValue}
        placeholder="Seleccionar"
        options={parsedOptions}
        disabled={campo.disable_campo === 1}
        data-ident={dataIdent}
        data-api-method={campo.apiMethod ?? undefined}
        aria-label={labelText}
        aria-describedby={tooltipId}
        aria-required={campo.obligatorio_campo === 1}
        title={titleText}
        onChange={(val) => {
          onChange?.(normalizeDynamicFieldValue(val), campo);
        }}
        onBlur={onBlur}
        onFocus={onFocus}
      />
    </Form.Item>
  );
}

export function CamposPlantillaAutoCompleteRenderer({
  camposPlantilla,
  className,
  fieldClassName,
  value,
  defaultValue,
  onChange,
  onBlur,
  onFocus,
  translate,
}: CamposPlantillaAutoCompleteRendererProps) {
  const camposFiltrados = camposPlantilla.filter(
    (campo) =>
      campo.campo_tip === 1 &&
      campo.name_campo !== "ASUNTO" &&
      (campo.ComportamientoCampo === "AUTOCOMPLETE" ||
        campo.ComportamientoCampo === "SELECCION"),
  );

  if (camposFiltrados.length === 0) {
    return null;
  }

  const campoKeyCount = new Map<string, number>();

  return (
    <Card
      data-ident="pl-radicacion-card-spe"
      className={`${styles.modernCard} ${className ?? ""}`.trim()}
      title={
        <Space className={styles.cardTitle}>
          <i className="fa-solid fa-database" />
          Datos Especializados
        </Space>
      }
      size="small"
      style={{ marginBottom: 24 }}
    >
      <Row gutter={16}>
        {camposFiltrados.map((campo) => {
          const dataGroup = campo.data_group ?? campo.TagSesion ?? undefined;
          const baseKey = [
            campo.name_campo || "campo-dinamico",
            campo.ComportamientoCampo || "sin-comportamiento",
            dataGroup || "sin-grupo",
            campo.tbl_control || "sin-tabla",
          ].join("|");
          const repeated = campoKeyCount.get(baseKey) ?? 0;
          campoKeyCount.set(baseKey, repeated + 1);
          const key = `${baseKey}|${repeated}`;
          if (campo.ComportamientoCampo === "SELECCION") {
            return (
              <Col key={key} xs={24} md={8} data-group={dataGroup}>
                <SelectField
                  campo={campo}
                  value={value?.[campo.name_campo]}
                  defaultValue={defaultValue?.[campo.name_campo]}
                  className={fieldClassName?.(campo)}
                  onChange={onChange}
                  onBlur={onBlur}
                  onFocus={onFocus}
                  translate={translate}
                />
              </Col>
            );
          }
          return (
            <Col key={key} xs={24} md={8} data-group={dataGroup}>
              <CampoPlantillaAutoCompleteField
                campo={campo}
                value={value?.[campo.name_campo]}
                defaultValue={defaultValue?.[campo.name_campo]}
                className={fieldClassName?.(campo)}
                onChange={onChange}
                onBlur={onBlur}
                onFocus={onFocus}
                translate={translate}
              />
            </Col>
          );
        })}
      </Row>
    </Card>
  );
}

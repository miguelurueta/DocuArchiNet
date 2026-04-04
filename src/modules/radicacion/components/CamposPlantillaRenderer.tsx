import { Card, Form, Input, Row, Col, Select, Space, Tooltip } from "antd";
import { InfoCircleOutlined } from "@ant-design/icons";
import type { FocusEventHandler } from "react";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import styles from "../style/FormRadicacion.module.css";

type CampoPlantillaEx = CampoPlantillaDTO & {
  data_group?: string | null;
};

interface CamposPlantillaRendererProps {
  camposPlantilla: ReadonlyArray<CampoPlantillaEx>;
  onChange?: (value: string, field: CampoPlantillaEx) => void;
  onBlur?: FocusEventHandler<HTMLInputElement | HTMLSelectElement>;
  onFocus?: FocusEventHandler<HTMLInputElement | HTMLSelectElement>;
  translate?: (value: string, field: CampoPlantillaEx) => string;
}

function getLabelText(
  field: CampoPlantillaEx,
  translate?: (value: string, field: CampoPlantillaEx) => string,
) {
  const raw = field.aleas_campo ?? field.name_campo;
  return translate ? translate(raw, field) : raw;
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

function getInputType(field: CampoPlantillaEx) {
  if (field.control_tip_correo === 1) {
    return "email";
  }
  const tipo = field.tipo_campo?.toLowerCase() ?? "";
  if (tipo.includes("fecha") || tipo.includes("date")) {
    return "date";
  }
  if (tipo.includes("numero") || tipo.includes("number") || tipo.includes("num")) {
    return "number";
  }
  return "text";
}

function getInputPattern(_field: CampoPlantillaEx, type: string) {
  if (type === "email") {
    return "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$";
  }
  if (type === "number") {
    return "^[0-9]+$";
  }
  return undefined;
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

export function CamposPlantillaRenderer({
  camposPlantilla,
  onChange,
  onBlur,
  onFocus,
  translate,
}: CamposPlantillaRendererProps) {
  const camposFiltrados = camposPlantilla.filter(
    (campo) => campo.campo_tip === 1,
  );
  const campoKeyCount = new Map<string, number>();

  return (
    <Card
      data-ident="pl-radicacion-card-spe"
      className={styles.modernCard}
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
          const labelText = getLabelText(campo, translate);
          const tooltipText = getTooltipText(campo, translate);
          const titleText = getTitleText(campo, translate);
          const controlId = `pl-radicacion-spe-${campo.name_campo}`;
          const tooltipId = tooltipText
            ? `pl-radicacion-spe-tooltip-${campo.name_campo}`
            : undefined;
          const required = campo.obligatorio_campo === 1;
          const disabled = campo.disable_campo === 1;
          const maxLength =
            typeof campo.max_leng_campo === "number" && campo.max_leng_campo > 0
              ? campo.max_leng_campo
              : undefined;
          const dataGroup = campo.data_group ?? campo.TagSesion ?? undefined;
          const baseKey = [
            campo.name_campo || "campo-dinamico",
            campo.ComportamientoCampo || "sin-comportamiento",
            dataGroup || "sin-grupo",
            campo.apiMethod || "sin-api",
          ].join("|");
          const repeated = campoKeyCount.get(baseKey) ?? 0;
          campoKeyCount.set(baseKey, repeated + 1);
          const key = `${baseKey}|${repeated}`;

          const ariaLabel = labelText;
          const ariaDescribedBy = tooltipId ?? undefined;

          const labelNode = (
            <span title={titleText} className={styles.labelCapitalize}>
              {labelText}
              {tooltipText ? (
                <Tooltip title={tooltipText}>
                  <span
                    className="tooltip-ayuda"
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

          if (campo.ComportamientoCampo === "SELECCION") {
            return (
              <Col
                key={key}
                xs={24}
                md={8}
                data-group={dataGroup}
              >
                <Form.Item
                  label={labelNode}
                  required={required}
                  data-ident={`pl-radicacion-spe-${campo.name_campo}`}
                >
                  <Select
                    id={controlId}
                    aria-label={ariaLabel}
                    aria-describedby={ariaDescribedBy}
                    aria-required={required}
                    data-ident={`pl-radicacion-spe-${campo.name_campo}`}
                    data-api-method={campo.apiMethod ?? undefined}
                    disabled={disabled}
                    maxLength={maxLength}
                    onChange={(value) => {
                      onChange?.(normalizeDynamicFieldValue(value), campo);
                    }}
                    onBlur={onBlur}
                    onFocus={onFocus}
                    options={(campo.ilist_row_drowlist ?? []).map((option) => ({
                      value: option.id_value,
                      label: option.value_campo,
                    }))}
                    title={titleText}
                  />
                </Form.Item>
              </Col>
            );
          }

          if (campo.ComportamientoCampo === "AUTOCOMPLETE") {
            const inputType = getInputType(campo);
            const pattern = getInputPattern(campo, inputType);
            return (
              <Col
                key={key}
                xs={24}
                md={8}
                data-group={dataGroup}
              >
                <Form.Item
                  label={labelNode}
                  required={required}
                  data-ident={`pl-radicacion-spe-${campo.name_campo}`}
                >
                  <Input
                    id={controlId}
                    name={campo.name_campo}
                    type={inputType}
                    pattern={pattern}
                    data-ident={`pl-radicacion-spe-${campo.name_campo}`}
                    data-api-method={campo.apiMethod ?? undefined}
                    required={required}
                    disabled={disabled}
                    maxLength={maxLength}
                    aria-label={ariaLabel}
                    aria-describedby={ariaDescribedBy}
                    onChange={(event) => {
                      onChange?.(normalizeDynamicFieldValue(event.target.value), campo);
                    }}
                    onBlur={onBlur}
                    onFocus={onFocus}
                    title={titleText}
                  />
                </Form.Item>
              </Col>
            );
          }

          return null;
        })}
      </Row>
    </Card>
  );
}

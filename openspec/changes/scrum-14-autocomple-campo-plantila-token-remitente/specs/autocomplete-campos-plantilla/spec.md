## MODIFIED Requirements

### Requirement: Autocompletado dinámico de campos de plantilla
El sistema SHALL renderizar campos con `campo_tip = 1` y `ComportamientoCampo` configurado para autocompletado dentro del `<Card data-ident="pl-radicacion-card-spe">`, usando un componente reutilizable que consulta la API de plantillas. Para el caso del control `data-ident="pl-radicacion-spe-REMITENTE_COR"`, el sistema SHALL resolver el registro correspondiente desde `camposPlantilla` comparando `name_campo = "REMITENTE_COR"` (normalizado por mayúsculas/minúsculas) y aplicar su metadata declarativa (`required`, `disabled`, `title`, `tooltipAyuda`, `aria-*`).

#### Scenario: Resuelve metadata de REMITENTE_COR por name_campo
- **WHEN** `camposPlantilla` contiene un elemento con `name_campo = "REMITENTE_COR"`
- **THEN** el campo `data-ident="pl-radicacion-spe-REMITENTE_COR"` usa ese registro de plantilla como fuente de configuración

#### Scenario: Renderiza autocompletado de REMITENTE_COR con atributos declarativos
- **WHEN** se renderiza el formulario de radicación con metadata válida para `REMITENTE_COR`
- **THEN** el control se muestra como autocompletado y preserva `required`, `disabled`, `title`, `tooltipAyuda`, `aria-label` y `aria-describedby`

### Requirement: Consulta a API de autocompletado
El sistema SHALL consultar `/api/PlantillaRadicado/autoCompleteTercero` para el campo `REMITENTE_COR`, enviando los parámetros requeridos por el endpoint para búsqueda incremental del tercero remitente. Las opciones devueltas SHALL mostrarse como sugerencias del autocompletado sin bloquear ingreso manual cuando no haya resultados.

#### Scenario: Invoca endpoint de tercero para REMITENTE_COR
- **WHEN** el usuario escribe en el autocompletado `REMITENTE_COR`
- **THEN** el frontend realiza la consulta a `/api/PlantillaRadicado/autoCompleteTercero` con el texto digitado y metadata del campo

#### Scenario: Muestra sugerencias devueltas por autoCompleteTercero
- **WHEN** la API responde con resultados de terceros para `REMITENTE_COR`
- **THEN** el autocompletado lista esas opciones para selección del usuario

#### Scenario: Fallback manual ante error o vacío
- **WHEN** la API de `autoCompleteTercero` responde con error o sin datos
- **THEN** el control mantiene ingreso manual y muestra manejo de error amigable sin romper el formulario

## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- OCR-ZONA. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-260.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> OCR POR ZONA (EXTRACCIÓN DE TEXTO DESDE ÁREA SELECCIONADA)
> CONTEXTO
> El módulo de digitalización permitirá seleccionar una región específica dentro de una página mediante la funcionalidad de Selección de Área.
> Se requiere aprovechar dicha selección para ejecutar OCR únicamente sobre la región seleccionada.
> OBJETIVO
> Permitir que el usuario seleccione una zona específica de una página y extraiga únicamente el texto contenido en dicha región.
> ==================================================
> DEPENDENCIA
> Requiere:
> SCRUMCORE-269Selección de Área + Recorte Manual
> La selección existente debe reutilizarse.
> NO crear un segundo mecanismo de selección.
> ==================================================
> TOOLBAR
> Agregar:
> [ OCR Zona ]
> Tooltip:
> "Extraer texto de la selección"
> ==================================================
> FLUJO
> Usuario selecciona una región.
> ↓
> Se habilita botón:
> OCR Zona
> ↓
> Ejecutar OCR únicamente sobre el área seleccionada.
> ↓
> Mostrar resultado.
> ==================================================
> RESULTADO
> Mostrar modal lateral o drawer:
> Texto extraído
> Ejemplo:
> 1122334455
> Acciones:
> [ Copiar ]
> [ Insertar en metadato ]
> [ Cerrar ]
> ==================================================
> CASOS DE USO
> Cédulas
> Extraer número de documento.
> Facturas
> Extraer:
> Número factura
> 
> Valor
> 
> Fecha
> 
> Contratos
> Extraer:
> Radicado
> 
> Número contrato
> 
> Formularios
> Extraer campos específicos.
> ==================================================
> EXPERIENCIA DE USUARIO
> Si no existe selección:
> Deshabilitar botón OCR Zona.
> Mostrar tooltip:
> "Seleccione un área primero"
> ==================================================
> PREPARACIÓN FUTURA
> Diseñar para soportar posteriormente:
> OCR múltiple.
> 
> OCR por varias regiones.
> 
> Extracción automática de metadatos.
> 
> IA documental.
> 
> ==================================================
> NO IMPLEMENTAR AÚN
> Antes de desarrollar:
> Auditar:
> Licencia actual de Dynamsoft.
> 
> Disponibilidad de OCR en licencia actual.
> 
> APIs OCR disponibles.
> 
> Idiomas soportados.
> 
> Rendimiento esperado.
> 
> ==================================================
> DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-277-ocr-zona.md
> Incluir:
> Arquitectura.
> 
> Flujo.
> 
> Dependencias.
> 
> Riesgos.
> 
> Casos de uso.
> 
> ==================================================
> VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> SOLO AUDITORÍA Y DISEÑO TÉCNICO.NO IMPLEMENTAR HASTA CONFIRMAR CAPACIDADES OCR DE LA LICENCIA ACTUAL.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, MODULOS, OCR, REUSABLE, ZONA

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-ocr-zona`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

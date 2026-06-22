## ADDED Requirements
### Requirement: Overlay Unificado De Progreso De Escaneo
El sistema SHALL mostrar un unico overlay corporativo para los estados de escaneo y procesamiento controlados por DocuArchi.

#### Scenario: Progreso durante adquisicion
- **WHEN** el usuario inicia un escaneo desde `DigitalizacionDocumentalWorkspace`
- **THEN** el preview muestra el estado `Escaneando documentos`
- **AND** el overlay no muestra barra de progreso, porcentaje, pagina actual ni mensajes tecnicos internos

#### Scenario: Progreso durante procesamiento controlado
- **WHEN** DocuArchi procesa paginas despues de la adquisicion
- **THEN** el overlay muestra `Procesando documentos` durante Deskew, Auto Crop, Auto Rotate, eliminacion de paginas en blanco, construccion de paginas y preparacion final
- **AND** el overlay muestra `Generando PDF` durante la generacion del PDF
- **AND** el footer reutiliza el mismo estado simplificado

#### Scenario: Limpieza de progreso
- **WHEN** el escaneo o la generacion PDF termina correctamente o falla
- **THEN** el overlay desaparece y el workspace vuelve al estado visual correspondiente

#### Scenario: Sin loaders duplicados
- **WHEN** `scanner.progress` existe o `scanner.loading` es verdadero
- **AND** el overlay corporativo esta visible en `Preview digitalizacion`
- **THEN** no se renderiza un spinner o loader historico adicional dentro del preview
- **AND** el overlay corporativo es la fuente unica de progreso visible

### Requirement: Auditoria De Limitaciones PaperStream
El sistema SHALL documentar que el dialogo nativo PaperStream IP no es personalizable desde React.

#### Scenario: Documentacion tecnica
- **WHEN** se revisa la documentacion de DigitalizacionDocumental
- **THEN** existe `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-275-scan-progress-modernization.md`
- **AND** el documento describe limitaciones del driver, eventos disponibles, estados soportados, diseno propuesto y riesgos.

### Requirement: Detalle funcional Jira
El sistema SHALL considerar las reglas detalladas del ticket.

#### Scenario: Reglas del ticket
- MODERNIZACIÓN DE EXPERIENCIA DE ESCANEO Y PROCESAMIENTO
- CONTEXTO
- La auditoría determinó que el diálogo:
- PaperStream IPEn digitalizaciónPágina XCancelar
- es renderizado por el driver nativo PaperStream IP y disparado por Dynamsoft Web TWAIN mediante AcquireImage().
- Por tanto:
- NO puede personalizarse desde React.
- NO puede modificarse visualmente desde DocuArchi.
- Sin embargo, DocuArchi sí controla completamente:
- Scanner Status
- 
- Preview PDF
- 
- Toolbar
- 
- Miniaturas
- 
- Overlay de carga
- 
- Procesamiento posterior
- 
- OBJETIVO
- Modernizar la experiencia visual controlada por DocuArchi.
- ==================================================
- FASE 1
- AUDITORÍA DE EVENTOS DYNAMSOFT
- Investigar si existen eventos disponibles para:
- Página adquirida
- 
- Página procesada
- 
- Avance de escaneo
- 
- Estado de adquisición
- 
- Determinar si puede obtenerse:
- Página actualTotal de páginas
- durante AcquireImage.
- ==================================================
- FASE 2
- NUEVO OVERLAY DOCUARCHI
- Crear overlay corporativo.
- Diseño:
- 📄 Escaneando documentos
- Página actual
- Barra de progreso
- Estado actual
- Cancelar operación
- ==================================================
- FASE 3
- ESTADOS SOPORTADOS
- Escaneando
- Procesando imágenes
- Aplicando Deskew
- Aplicando Auto Crop
- Aplicando Auto Rotate
- Eliminando páginas en blanco
- Generando PDF
- Preparando documento
- ==================================================
- FASE 4
- ELIMINAR DUPLICIDAD VISUAL
- Actualmente existen:
- Loader Preview
- 
- Indicadores dispersos
- 
- Unificar experiencia.
- Mostrar un único estado visual consistente.
- ==================================================
- FASE 5
- OPTIMIZACIÓN DE VELOCIDAD PERCIBIDA
- Evaluar:
- Render bloqueante
- 
- Actualización de miniaturas
- 
- Regeneración de preview
- 
- Reconstrucción de páginas
- 
- Documentar oportunidades de mejora.
- ==================================================
- FASE 6
- DOCUMENTACIÓN
- Crear:
- docs/Architecture/DigitalizacionDocumental/SCRUMCORE-275-scan-progress-modernization.md
- Incluir:
- Resultado auditoría PaperStream.
- 
- Limitaciones del driver.
- 
- Eventos disponibles.
- 
- Diseño propuesto.
- 
- Mockups.
- 
- Riesgos.
- 
- ==================================================
- VALIDAR
- npx tsc --noEmit
- eslint
- vitest
- IMPLEMENTAR

## 1. Setup

- [ ] 1.1 Confirmar rutas del visor y modal existentes
- [ ] 1.2 Localizar axios instance / `baseURL` y helper de JWT (sin exponer tokens)
- [ ] 1.3 Definir tipos TS del contrato SCRUM-201 (wrapper + DTO)

## 2. API Integration (firma temporal)

- [ ] 2.1 Implementar servicio/hook `useWorkflowPersonalSignature` (metadata + download)
- [ ] 2.2 Implementar regla `UrlTemporal` (absoluta vs relativa) sin parsear token
- [ ] 2.3 Implementar reintento 404: re-metadata + 1 retry download
- [ ] 2.4 Implementar lifecycle correcto de `ObjectURL` (revoke en cleanup/reemplazo)
- [ ] 2.5 Mapear wrapper de errores (`success/message/errors[]`) a estados enterprise sin `any`

## 3. UI (Modal “Firma personal”)

- [ ] 3.1 Agregar tab “Firma personal” en `AppPdfSignatureModal.tsx`
- [ ] 3.2 Renderizar estados enterprise (loading/empty/error/ready)
- [ ] 3.3 Mostrar metadata mínima (filename + expiración cuando exista)
- [ ] 3.4 Acción “Usar firma personal” reutiliza pipeline existente del modal
- [ ] 3.5 Validar que no hay wrappers extra ni lógica en `DocumentosWorkbench`
- [ ] 3.6 Botón “Reintentar” limpia estado previo y revoca `ObjectURL` si existía

## 4. Tests (Vitest + RTL)

- [ ] 4.1 Test: pestaña “Firma personal” renderiza en modal
- [ ] 4.2 Test: `loading` → `ready` con mocks (metadata + blob)
- [ ] 4.3 Test: `success=true` con `data=null` produce `empty`
- [ ] 4.4 Test: `download 404` dispara reintento 1 vez (metadata + download)
- [ ] 4.5 Test: cleanup revoca `ObjectURL` al desmontar/cerrar

## 5. Documentación enterprise (SCRUMCORE-211)

- [ ] 5.1 Crear `SCRUMCORE-211-Metadata.md` (branch/commits/tests/evidencias)
- [ ] 5.2 Crear `SCRUMCORE-211-APIs-Utilizadas.md` (contrato SCRUM-201 + reglas UrlTemporal)
- [ ] 5.3 Crear `SCRUMCORE-211-Comportamiento-del-Componente.md` (tabs + estados)
- [ ] 5.4 Crear `SCRUMCORE-211-Arquitectura-Tecnica.md` (flujo + Mermaid)
- [ ] 5.5 Crear `SCRUMCORE-211-Testing-Enterprise.md` (unit + evidencias)
- [ ] 5.6 Incluir Mermaid del flujo `UrlTemporal` + retry 404 + lifecycle de `ObjectURL`

# PROMPT ARQUITECTONICO  Ticket 04 BE
# Contrato backend de metadata de archivos

Rol esperado:
Arquitecto de software senior backend (APIs enterprise, seguridad, contratos)


OBJETIVO

Definir el contrato backend para persistir metadata de archivos cargados, alineado a AppUpload.


CONTEXTO EXISTENTE

- arquitectura: `docs/Architecture/AppUpload/AppUpload-Architecture.md`
- endpoint futuro definido: `POST /files/metadata`


UBICACION (OBLIGATORIA)

```
Backend / Documentacion de API
```


RESTRICCIONES (OBLIGATORIAS)

- contrato versionable
- no acoplar a storage especifico


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

- endpoint: `POST /files/metadata`
- payload: lista de archivos con `name`, `size`, `type`, `url`, `hash?`, `owner?`, `tags?`
- contexto: `modulo`, `entidadId`, `tenantId`
- respuesta: ids persistidos + estado por archivo
- documentar errores 4xx/5xx


CRITERIOS DE ACEPTACION

- contrato claro y alineado a AppUpload
- response usable por FE sin transformaciones adicionales

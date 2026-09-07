# Arquitectura

Preflight revalida y produce un plan sin efectos. El servicio de intención crea una huella canónica; el guard limita concurrencia y el repositorio conserva cabecera, requisitos y elementos en una transacción. La restricción única MySQL es la autoridad final.

No se modifican ASMX, Session, caché SII, almacenamiento ni tablas legacy.

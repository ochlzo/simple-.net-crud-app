column name: riser
attributes:
riser_id PK
riser_name
site_id FK to site

column name: site
attributes:
site_id PK
site_name
owner_id FK to site_owner

column name: site_owner
attributes:
owner_id PK
owner_name
owner_email
password

column name: room
attributes:
room_id PK
room_name
riser_id FK to riser
tenant_id FK to tenant

column name: tenant
attributes:
tenant_id PK
tenant_name

column name: document
attributes:
document_id PK
doc_name
tenant_id FK to tenant

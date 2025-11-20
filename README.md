# Contract Monthly Claim System (CMCS)    
PROG6212 – POE Part 3 | 2025

## Project Overview
A secure, role-based web application for managing lecturer hourly claims with verification, approval, and payment processing workflow.

## Power Point Presentation &  Live Demo (YouTube Video)  
Watch the full  presentation & live walkthrough:  
[youtube ](https://youtu.be/gjHO3TS-df0) 



## GitHub Repository  
[Link](https://github.com/VCCT-PROG6212-2025-G3/ST10307204_CMCS_PROG6212_POE)  


### Roles & Features
| Role             | Key Features |
|------------------|------------|
| **Lecturer**     | Submit claims with drag & drop, track status with progress bar |
| **Coordinator** | Verify claims via modal (no mis-clicks), view/download documents |
| **Manager**      | Final approval in modal, full visibility |
| **HR**           | View all claims, generate individual payment invoices (PDF) |

## Lecturer Feedback – Fully Implemented
| Feedback | Implementation |
|---------|----------------|
| Upload documents during claim creation | All supporting documents uploaded in **one go** – no separate step |
| Use modal pop-ups for verification/approval | Modal with blurred background, action buttons inside |
| Drag & drop document upload | Beautiful drag zone with live preview and validation |

## Key Features
- Drag & drop file upload with preview
- Modal-based verification & approval (Coordinator/Manager)
- Progress bar in Track Claim
- Individual professional PDF invoices (QuestPDF)
- Responsive Bootstrap 5 design
- Role-based security with session management
- Secure file storage in `wwwroot/uploads`

## Technologies
- ASP.NET Core 8 MVC
- Entity Framework Core
- Bootstrap 5
- QuestPDF
- Font Awesome
- Git & GitHub (10+ meaningful commits)

## Setup
1. Clone repository
2. Update `appsettings.json` connection string
3. Run project

## License
Community use – QuestPDF Community License applied  
`QuestPDF.Settings.License = LicenseType.Community;`

**Submitted by:** Luis Jose De Sousa Junior  
**Student Number:** ST10307204 
20th November 2025

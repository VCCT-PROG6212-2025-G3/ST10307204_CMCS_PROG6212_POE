
# CMCS – Contract Monthly Claim System  
**PROG6212 POE – Part 2  README File**  
*Streamlining claim submission, verification, and approval for independent contractor lecturers*

---

## Table of Contents
1. [Overview](#overview)  
2. [Changes from Part 1 to Part 2 (Lecturer Feedback)](#changes-from-part-1-to-part-2-lecturer-feedback)  
3. [Features](#features)  
4. [Technology Stack](#technology-stack)  
5. [Prerequisites](#prerequisites)  
6. [Installation & Setup](#installation--setup)  
7. [Running the Application](#running-the-application)  
8. [Running Unit Tests](#running-unit-tests)  
9. [User Roles & Workflows](#user-roles--workflows)  
10. [YouTube Video Demonstration](#youtube-video-demonstration)  
11. [Security Notes](#security-notes)   
12. [References](#references)  

---

## Overview
The **Contract Monthly Claim System (CMCS)** is an ASP.NET Core MVC web application designed to automate the submission, verification, and approval of monthly claims for independent contractor lecturers. The system supports encrypted document handling, role-based workflows, and real-time feedback. This is the **final Part 2 submission**, incorporating **all feedback** from Part 1.

---

## Changes from Part 1 to Part 2 (Lecturer Feedback)
The following improvements were made **in direct response to feedback** received on **Part 1**:

| Feedback Area | Issue in Part 1 | Changes in Part 2 |
|---------------|------------------|-------------------|
| **UML Class Diagram** (14/20) | Missing Coordinator/Manager entities, no methods or relationships shown | Added **Coordinator** and **Manager** classes with attributes and methods (`VerifyClaim()`, `ApproveClaim()`). Introduced **Approval** entity linking to **Claim**. UML updated in `documentation/UML_Part2.png`. |
| **GUI/UI Design** (22/25) | Lecturer dashboard too compact; submission and tracking not separated | Split into **SubmitClaim** and **TrackClaims** views. Added **statistics panel** (total claims, pending, verified, approved). Improved spacing and visual hierarchy. |

> **All feedback addressed.** System is now **more maintainable, testable, and user-friendly**.

---

## Features

| Role | Key Functionality |
|------|-------------------|
| **Lecturer** | • Submit claim with personal details<br>• Upload encrypted `.pdf|.docx|.xlsx` (max 10 MB)<br>• Real-time document list via AJAX |
| **Coordinator** | • View pending claims<br>• Verify/Reject<br>• Download decrypted documents |
| **Manager** | • View verified claims<br>• Approve/Reject<br>• Download decrypted documents |
| **All** | • AES-encrypted file storage<br>• Flash messages<br>• Responsive Bootstrap UI |

---

## Technology Stack
- **.NET 8.0**  
- **ASP.NET Core MVC**  
- **xUnit + Moq** for unit testing  
- **Bootstrap 5 + Font Awesome**  
- **System.Security.Cryptography (AES)**  

---

## Prerequisites
- **Visual Studio 2022** (Community/Professional) with **ASP.NET and web development** workload  
- **.NET 8 SDK**  
- **Git** (optional)  
- **7-Zip or WinRAR** to extract `.zip`

---

## Installation & Setup



### Step 1: Open in Visual Studio
1. Open `CMCS_PROG6212_POE.sln` in **Visual Studio 2022**.  
2. Restore NuGet packages: **Right-click Solution → Restore NuGet Packages**.

### Step 2: Add Test Project (from ZIP)
1. Extract `CMCS_PROG6212_POE.Tests.zip` to a folder (e.g., `C:\Projects\CMCS_PROG6212_POE.Tests`).  
2. In Visual Studio:  
   - **Right-click Solution → Add → Existing Project**  
   - Navigate to extracted folder → Select `CMCS_PROG6212_POE.Tests.csproj`  
3. Build the test project: **Right-click test project → Build**.

### Step 4: Create Uploads Folder

mkdir wwwroot\uploads

> **Windows**: Grant **Modify** permission to `IIS_IUSRS` on `wwwroot/uploads`.

---

## Running the Application
1. Set `CMCS_PROG6212_POE` as **Startup Project**.  
2. Press **F5** or **Debug > Start Debugging**.  
3. Open browser at `https://localhost:7xxx` (port shown in console).

---

## Running Unit Tests

###  Visual Studio Test Explorer
1. Open **Test > Test Explorer**.  
2. Click **Run All**.  
3. All **5 tests** should pass.



**Tests Covered:**  
1. `SubmitClaim_ValidModel_ReturnsSuccess`  
2. `UploadDocuments_ValidFiles_ReturnsSuccess`  
3. `Index_VerifyClaim_UpdatesStatus`  
4. `Index_ApproveClaim_UpdatesStatus`  
5. `GetDocument_ValidRequest_ReturnsFile`

---

## User Roles & Workflows
1. **Lecturer**: Submit → Upload → Track (with stats)  
2. **Coordinator**: Verify/Reject → Download  
3. **Manager**: Approve/Reject → Download  

---

## YouTube Video Demonstration
**[Watch the Full System Demo on YouTube]**  
Link: [[https://youtu.be/at8f8LEIlik](https://youtu.be/at8f8LEIlik)]  


**Video covers:**  
- Claim submission & upload  
- Coordinator verification  
- Manager approval  
- Unit test execution  

---

## Security Notes
- Files encrypted using **AES-256** with key `"16-char-key-1234"`  
- **Replace key in production**  
- No authentication — **add ASP.NET Identity** for deployment  
- Encrypted files stored in `wwwroot/uploads` (outside web root access)

---


## References
- Microsoft (2024) *ASP.NET Core MVC Documentation*. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc (Accessed: 22 October 2025).  
- Microsoft (2024) *System.Security.Cryptography.Aes Class*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes (Accessed: 22 October 2025).  
- Troelsen, A. and Japikse, P. (2021) *Pro C# 10 with .NET 6*. Apress.  
- xUnit.net (2024) *xUnit Documentation*. Available at: https://xunit.net (Accessed: 22 October 2025).  
- Moq (2024) *Moq Framework*. Available at: https://github.com/moq/moq (Accessed: 22 October 2025).
- Grok (2025) AI Assistant for Software Development and Debugging. Developed by xAI. Available at: https://grok.x.ai (Accessed: 22 October 2025). 

---


**Prepared for PROG6212 POE – Part 2 Submission – 22 October 2025**  
*All Part 1 feedback implemented. System fully functional, tested, and documented.*
---

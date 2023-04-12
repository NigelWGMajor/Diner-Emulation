# General shell for emulator

A shell for housing simple projects.
Some simple demo cases are included:

- Log viewer
- Chart
- Paging/Nav buttons
- hopefully some more generics will be added.

Razor pages & Js at te FE
C# at the BE


Uses signalR to pulse the FE.

Startup with pages:

Emulator = Index
Monitor 
Setting 
About
Error

Most pages have 2 buttons, 
- one is a page redirect (test error from XXX) that passes a parameter to the error page
- the other posts on-page to a local handler

Monitor page has a chart fed by SignalR, implemented in JS and an Event log likewise.

Settings page has a few controls bound to the code-behind on submission. Should use some service for persistence.

About page is boringly empty.




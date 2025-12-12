namespace TuitionManagementSystem.Web.Features.Abstractions;

using System.Net.Mime;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
[TranslateResultToActionResult]
[Produces(MediaTypeNames.Application.Json)]
public class ApiController : ControllerBase;

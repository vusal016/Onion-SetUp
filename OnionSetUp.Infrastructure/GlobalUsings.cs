global using OnionSetUp.Application.Common.Interfaces;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using OnionSetUp.Application.Common.Constants;
global using OnionSetUp.Application.Common.Identity;
global using OnionSetUp.Application.Services.Abstractions;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Options;
global using OnionSetUp.Domain.Models;
global using OnionSetUp.Infrastructure.Identity.Settings;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using OnionSetUp.Infrastructure.Services;
global using AutoMapper;
global using OnionSetUp.Application.Common.Dtos.ProfileDtos;
global using OnionSetUp.Domain.Exceptions;
global using Microsoft.EntityFrameworkCore;
global using OnionSetUp.Application.Common.Dtos.ServiceDtos;

    
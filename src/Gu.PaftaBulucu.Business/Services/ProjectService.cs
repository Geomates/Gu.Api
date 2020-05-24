using Gu.PaftaBulucu.Business.Dtos;
using Gu.PaftaBulucu.Data.Models;
using Gu.PaftaBulucu.Data.Repositories;
using Nelibur.ObjectMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Business.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IDatabaseRepository<Project> _projectRepository;

        public ProjectService(IDatabaseRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ListProjectDto>> GetProjects(string email)
        {
            TinyMapper.Bind<List<Project>, List<ListProjectDto>>();
            var projects = await _projectRepository.FindAsync(p => p.Email == email);
            return TinyMapper.Map<List<ListProjectDto>>(projects);
        }

        public async Task<int> SaveProject(SaveProjectDto projectDto)
        {
            TinyMapper.Bind<SaveProjectDto, Project>();
            var project = TinyMapper.Map<Project>(projectDto);

            await _projectRepository.AddAsync(project);

            await _projectRepository.CommitAsync();

            return project.ProjectId;
        }

        public async Task SaveProject(int projectId, SaveProjectDto projectDto)
        {
            TinyMapper.Bind<SaveProjectDto, Project>();

            var project = await _projectRepository.GetByIdAsync(projectId);

            project = TinyMapper.Map(projectDto, project);

            _projectRepository.NotifyChange(project);

            await _projectRepository.CommitAsync();
        }

        /*
        TODO: DELETE AFTER MIGRATION

        private readonly Dictionary<int, int> _scaleRanges = new Dictionary<int, int>
        {
            {100, 18000000},
            {50, 9000000},
            {25, 4500000},
            {10, 1800000},
            {5, 900000},
            {2, 450000},
            {1, 225000}

        };

        public async Task MigrateProjects()
        {
            var projects = await _projectRepository.GetAllAsync();
            var sheetNameRegex =
                new Regex(@"^([a-zA-ZsçÇöÖşŞıİğĞüÜ]*?\-?[A-Z]\s[0-9]{2})\-?([abcd])?([1-4])?\-?([0-9]{1,2})?\-?([abcd])?\-?([1-4])?\-?([abcd])?");
            
            foreach (var project in projects)
            {
                foreach (var sheetEntry in project.Entries)
                {
                    var name = sheetEntry.Name.Split('-').Select(s => s.Trim());
                    sheetEntry.Name = string.Join("-", name);
                    var matchedSheetParts = sheetNameRegex.Match(sheetEntry.Name);
                    if (matchedSheetParts.Success)
                    {
                        var matchCount = matchedSheetParts.Groups.Values.Count(v => v.Value.Length > 0);
                        matchCount += matchCount < 4 || matchCount == 4 && matchedSheetParts.Groups[3].Value.Length > 0 ? -2 : -1;
                        var newScale = _scaleRanges.Keys.ToList()[matchCount];
                        if (sheetEntry.Scale != newScale)
                        {
                            sheetEntry.Scale = newScale;
                            _projectRepository.NotifyChange(project);
                        }
                    }


                    if (sheetEntry.Lat > 180)
                    {
                        sheetEntry.Lat /= 36000000;
                    }

                    if (sheetEntry.Lng > 180)
                    {
                        sheetEntry.Lng /= 36000000;
                    }
                }
               
                await _projectRepository.CommitAsync();
            }
        }
        */
    }
}

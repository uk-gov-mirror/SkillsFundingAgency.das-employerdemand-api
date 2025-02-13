using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerDemand.Domain.Entities;
using SFA.DAS.EmployerDemand.Domain.Interfaces;

namespace SFA.DAS.EmployerDemand.Data.Repository
{
    public class CourseDemandRepository : ICourseDemandRepository
    {
        private readonly ILogger<CourseDemandRepository> _logger;
        private readonly IEmployerDemandDataContext _dataContext;

        public CourseDemandRepository (ILogger<CourseDemandRepository> logger, IEmployerDemandDataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<bool> Insert(CourseDemand item)
        {
            try
            {
                await _dataContext.CourseDemands.AddAsync(item);
                _dataContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException e)
            {
                _logger.LogInformation(e, "Unable to add course demand item");
            }
            return false;
        }

        public async Task<IEnumerable<AggregatedCourseDemandSummary>> GetAggregatedCourseDemandList(int ukprn, int? courseId, double? lat, double? lon, int? radius)
        {
            var result = _dataContext.AggregatedCourseDemandSummary.FromSqlInterpolated(ProviderCourseDemandQuery(lat,lon, radius)).AsQueryable();

            if (courseId != null)
            {
                result = result.Where(c => c.CourseId.Equals(courseId)).AsQueryable();
            }
            
            return await result.OrderBy(c=>c.CourseTitle).ToListAsync();
        }

        public async Task<int> TotalCourseDemands(int ukprn)
        {
            var value = await _dataContext.CourseDemands.GroupBy(c => c.CourseId).CountAsync();
            
            return value;
        }

        private FormattableString ProviderCourseDemandQuery(double? lat, double? lon, int? radius)
        {
            return $@"select distinct
                        c.CourseId,
                        c.CourseTitle,
                        c.CourseLevel,
                        c.CourseRoute,
                        derv.ApprenticesCount,
                        derv.EmployersCount,
                        derv.DistanceInMiles
                    from CourseDemand c
                    inner join (
                        select
                            cd.CourseId,
                            Sum(NumberOfApprentices) as ApprenticesCount,
                            Count(1) as EmployersCount,
                            Max(dist.DistanceInMiles) as DistanceInMiles
                        from CourseDemand cd
                    inner join(
                        select
                            Id,
                            courseId,
                            geography::Point(isnull(Lat,0), isnull(Long,0), 4326).STDistance(geography::Point(isnull({lat},0), isnull({lon},0), 4326)) * 0.0006213712 as DistanceInMiles

                        from CourseDemand) as dist on dist.Id = cd.Id and ({radius} is null or (DistanceInMiles < {radius}))
                    Group by cd.CourseId ) derv on derv.CourseId = c.CourseId";
        }
    }
}

﻿namespace core.service
{
    using Microsoft.Extensions.Logging;

    public class DemoService : IDemoService
    {
        private readonly ILogger<DemoService> logger;

        public DemoService(ILogger<DemoService> logger)
        {
            this.logger = logger;
        }

        public void DoSomething()
        {
            logger.LogInformation("Demo");
        }
    }
}
